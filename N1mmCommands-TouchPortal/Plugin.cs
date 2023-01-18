/* This project published at https://github.com/frazierjason/N1mmCommands-TouchPortal
 * under the MIT license.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TouchPortalExtension.Attributes;
using TouchPortalSDK;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Events;
using TouchPortalSDK.Messages.Models;

namespace N1mmCommands.Touchportal
{
    public class Plugin : ITouchPortalEventHandler
    {
        private const string APP_NAME = "N1MM+ Commands for TouchPortal";
        private const string N1MM_RADIOINFO_BROADCAST_ADDRESS = "N1MM+ RadioInfo Broadcast Address";
        private const string N1MM_UDP_BROADCAST_PORT = "N1MM+ RadioInfo Broadcast Port";
        private const string N1MM_RADIOCMD_LISTENER_ADDRESS = "N1MM+ RadioCmd Listener Address";
        private const string N1MM_UDP_LISTENER_PORT = "N1MM+ RadioCmd Listener Port";

        private readonly byte[] N1MM_RADIOCMD_PREFIX_FOR_RADIO1 = System.Text.Encoding.ASCII.GetBytes("<RadioCmd><RadioNr>1</RadioNr><FocusEntry>");
        private readonly byte[] N1MM_RADIOCMD_PREFIX_FOR_RADIO2 = System.Text.Encoding.ASCII.GetBytes("<RadioCmd><RadioNr>2</RadioNr><FocusEntry>");
        private readonly byte[] N1MM_RADIOCMD_MIDFIX = System.Text.Encoding.ASCII.GetBytes("</FocusEntry><RadioCommand>");
        private readonly byte[] N1MM_RADIOCMD_SUFFIX = System.Text.Encoding.ASCII.GetBytes("</RadioCommand></RadioCmd>");

        private bool _isClosing;
        private System.Net.Sockets.Socket _commandSock;
        private System.Net.IPEndPoint? _commandSockEndpoint = null;
        private bool _shouldReconfigureN1mmSockets;
        private System.Net.IPAddress? _n1mmRadioInfoBroadcastIP = null;
        private System.Net.IPAddress? _n1mmRadioCmdListenerIP = null;
        private int _n1mmRadioInfoBroadcastPort = 0;
        private int _n1mmRadioCmdListenerPort = 0;

        public string PluginId => "n1mm.commands.tp";

        private readonly ILogger<Plugin>? _logger;
        private readonly ITouchPortalClient _client;

        private RadioInfo[] _radioInfo = new RadioInfo[2];  // track radio1, radio2
        private ushort _currentRadioIdx = 0;  // start with radio1
        private bool _hasSecondRadio = false;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int GetForegroundWindow();
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(int hWnd);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ShowWindow(int hWnd, int nCmdShow);
        private const int SW_RESTORE = 9;

        private readonly InputSimulatorEx.InputSimulator _sim = new();

        public Plugin(ITouchPortalClientFactory clientFactory, ILogger<Plugin> logger)
        {
            _logger = logger ?? null;
            _logger?.LogInformation("Plugin(): Created client");

            // our radios start out empty, so set them as invalid for use
            _radioInfo[0] = new RadioInfo();
            _radioInfo[0].invalidate();
            _radioInfo[1] = new RadioInfo();
            _radioInfo[1].invalidate();

            _commandSock = new System.Net.Sockets.Socket(
                System.Net.Sockets.AddressFamily.InterNetwork,
                System.Net.Sockets.SocketType.Dgram,
                System.Net.Sockets.ProtocolType.Udp);

            _client = clientFactory.Create(this);
        }

        ~Plugin()
        {
            _commandSock.Close();
        }

        private void serializer_UnknownNode(object? sender, System.Xml.Serialization.XmlNodeEventArgs? e)
        {
            _logger?.LogDebug($"serializer_UnknownNode(): got unknown RadioInfo node: {e?.Name ?? "<null>"}, type {e?.Text ?? "<null>"}\n");
        }
        private void serializer_UnknownAttribute(object? sender, System.Xml.Serialization.XmlAttributeEventArgs? e)
        {
            System.Xml.XmlAttribute? attr = e?.Attr ?? null;
            _logger?.LogDebug($"serializer_UnknownAttribute(): got unknown RadioInfo attribute: {attr?.Name ?? "<null>"}, type {attr?.Value ?? "<null>"}\n");
        }

        public void Run()
        {
            if (!_client.Connect())
            {
                _logger?.LogError("Run(): _client failed to connect to TouchPortal app. Exiting.");
                _isClosing = true;
                _commandSock?.Close();
                Environment.Exit(1);
            }
            _logger?.LogInformation("Run(): Connected to TouchPortal. Wait for settings before starting to receive N1MM+ messages...");

            _client.StateUpdate("n1mm.states.radioConnectionState", "Awaiting N1MM+");

            _shouldReconfigureN1mmSockets = true;
            while (null == _n1mmRadioInfoBroadcastIP || 0 == _n1mmRadioInfoBroadcastPort || 0 == _n1mmRadioCmdListenerPort)
            {
                // wait up to 20 secoonds for all our settings to arrive from TouchPortal
                for (int i = 20; i > 0; --i)
                {
                    System.Threading.Thread.Sleep(50);
                    if (null != _n1mmRadioInfoBroadcastIP && 0 != _n1mmRadioInfoBroadcastPort && 0 != _n1mmRadioCmdListenerPort)
                    {
                        break;
                    }
                    if (true == _isClosing)
                    {
                        _commandSock?.Close();
                        Environment.Exit(1);
                    }
                }
                _logger?.LogDebug("Run(): Still waiting for settings from TouchPortal before starting to listen for N1MM+ messages...");
            }

            _shouldReconfigureN1mmSockets = false;
            ReceiveMessage(_n1mmRadioInfoBroadcastPort, _logger);
        }

        private void ReceiveMessage(int port, ILogger<Plugin>? logger)
        {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            Task.Run(async () =>
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            {
                while (true)
                {
                    logger?.LogTrace("ReceiveMessage(): create udpClient\n");
                    System.Net.Sockets.UdpClient udpClient = new System.Net.Sockets.UdpClient();
                    logger?.LogTrace("ReceiveMessage(): set reuseAddress\n");
                    udpClient.Client.SetSocketOption(
                        System.Net.Sockets.SocketOptionLevel.Socket,
                        System.Net.Sockets.SocketOptionName.ReuseAddress, true);

                    logger?.LogTrace("ReceiveMessage(): set broadcast\n");
                    udpClient.Client.SetSocketOption(
                        System.Net.Sockets.SocketOptionLevel.Socket,
                        System.Net.Sockets.SocketOptionName.Broadcast, true);

                    logger?.LogTrace("ReceiveMessage(): clear exclusiveaddressuse\n");
                    udpClient.ExclusiveAddressUse = false;

                    logger?.LogInformation("ReceiveMessage(): bind our N1MM+ client to receive from N1MM+ at " + 
                        "${_n1mmRadioInfoBroadcastIP ?? System.Net.IPAddress.Any}:{_n1mmRadioInfoBroadcastPort}...\n");
                    System.Net.IPEndPoint n1mmBroadcastEndpoint = new System.Net.IPEndPoint(
                        _n1mmRadioInfoBroadcastIP ?? System.Net.IPAddress.Any, _n1mmRadioInfoBroadcastPort);
                    udpClient.Client.Bind(n1mmBroadcastEndpoint);

                    logger?.LogInformation("ReceiveMessage(): Entering ReceiveMessage() wait loop for N1MM+ to send us any UDP datagrams\n");
                    while (false == _shouldReconfigureN1mmSockets)  // break out of inner loop when a reconnection is needed as detected elsewhere
                    {
                        byte[] receivedResult = Array.Empty<byte>();

                        // during the forever loop waiting to get UDP messages, check if we have no usable radioInfo objects roughly every 100mS
                        // or so, which works out to once every ten times through the loop.  radioInfo objects are not usable if they are null,
                        // or if the last time they were updated was >15 seconds ago (informal agreement with N1MM+ dev team).  clean up any
                        // stale radios.
                        ushort lazyWatchdogCounter = 0;

                        // https://stackoverflow.com/questions/5932204/c-sharp-udp-listener-un-blocking-or-prevent-revceiving-from-being-stuck
                        while (true)
                        {
                            try
                            {
                                if (true == _shouldReconfigureN1mmSockets)
                                {
                                    break;
                                }
                                if (udpClient.Client.IsBound && udpClient.Available > 0)
                                {
                                    receivedResult = udpClient.Receive(ref n1mmBroadcastEndpoint);
                                    break;
                                }
                                System.Threading.Thread.Sleep(10); // wait a little, keep our CPU usage down
                                // watchdog wakes up every tenth cycle (100+ mS), and checks for staleness if we have at least one radioInfo object
                                //
                                // the watchdog also takes care of cleaning up radio states and resyncing the state with TP as needed
                                if (++lazyWatchdogCounter > 9)
                                {
                                    lazyWatchdogCounter = 0;
                                    long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                                    if (!_radioInfo[0].isInvalidated)
                                    {
                                        if (now - _radioInfo[0].refreshTime > 15000)
                                        {
                                            _radioInfo[0].invalidate();
                                            _radioInfo[1].invalidate();
                                            _hasSecondRadio = false;
                                            _currentRadioIdx = 0;
                                            _client.StateUpdate("n1mm.states.radioConnectionState", "Awaiting N1MM+");
                                            logger?.LogInformation("ReceiveMessage(): waited >15 seconds for N1MM+ to send us more UDP datagrams, no radios connected\n");
                                        }
                                        else
                                        {
                                            // we still have an alive Radio 1, so let's now check Radio 2
                                            if (!_radioInfo[1].isInvalidated)
                                            {
                                                if (now - _radioInfo[1].refreshTime > 15000)
                                                {
                                                    _radioInfo[1].invalidate();
                                                    _hasSecondRadio = false;
                                                    _currentRadioIdx = 0;
                                                    _client.StateUpdate("n1mm.states.radioConnectionState", "Radio 1");
                                                    logger?.LogInformation("ReceiveMessage(): Radio 2 sent no data for >15 seconds, marking as disconnected\n");
                                                }
                                                else
                                                {
                                                    // and we still have an alive Radio 2
                                                    if (!_hasSecondRadio)
                                                    {
                                                        _hasSecondRadio = true;
                                                        // use the freshest radio's data
                                                        int activeRadio = (_radioInfo[0].refreshTime > _radioInfo[1].refreshTime ? _radioInfo[0] : _radioInfo[1]).ActiveRadioNr;
                                                        _client.StateUpdate("n1mm.states.radioConnectionState", $"Radio {activeRadio} of 2");
                                                        logger?.LogDebug("ReceiveMessage(): set an unexpectedly cleared _hasSecondRadio flag when Radio 2 was already init'd\n");
                                                    }
                                                }
                                            }
                                            else if (_hasSecondRadio)
                                            {
                                                _hasSecondRadio = false;
                                                logger?.LogDebug("ReceiveMessage(): cleared an unexpected _hasSecondRadio flag when Radio 2 was already invalidated\n");
                                            }
                                        }
                                    }
                                    else if (_hasSecondRadio || !_radioInfo[1].isInvalidated)
                                    {
                                        _radioInfo[1].invalidate();
                                        _hasSecondRadio = false;
                                        _currentRadioIdx = 0;
                                        _client.StateUpdate("n1mm.states.radioConnectionState", "Radio 1");
                                        logger?.LogDebug("ReceiveMessage(): cleaned up Radio 2 or _hasSecondRadio unexpectedly when Radio 1 was already invalidated\n");
                                    }
                                }
                            }
                            catch
                            {
                                // something bad happened, clean up the socket and get a new one until we have more exceptional finesse here
                                udpClient.Close();
                                _shouldReconfigureN1mmSockets = true;

                                _radioInfo[0].invalidate();
                                _radioInfo[1].invalidate();
                                _hasSecondRadio = false;
                                _client.StateUpdate("n1mm.states.radioConnectionState", "Awaiting N1MM+");
                            }
                        }
                        if (true == _shouldReconfigureN1mmSockets)
                        {
                            // don't attempt to read any (more) UDP data even if we have some bytes, since it's time to change the socket out
                            // our radio info will be invalid, so purge it all now
                            continue;
                        }

                        System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(RadioInfo));

                        if (null == receivedResult || receivedResult.Length == 0)
                        {
                            logger?.LogTrace($"ReceiveMessage(): Received null or empty message byte array\n");
                            continue;
                        }
                        else if (!(System.Text.Encoding.ASCII.GetString(receivedResult).Contains("<RadioInfo>")))
                        {
                            logger?.LogTrace($"ReceiveMessage(): Received non-RadioInfo message data:\n,{System.Text.Encoding.ASCII.GetString(receivedResult)}\n");
                            continue;
                        }

                        System.IO.MemoryStream? ms = null;
                        RadioInfo? newRadioInfo = null;
                        int newRadioIdx = 0;
                        try
                        {
                            serializer.UnknownNode += new System.Xml.Serialization.XmlNodeEventHandler(serializer_UnknownNode);
                            serializer.UnknownAttribute += new System.Xml.Serialization.XmlAttributeEventHandler(serializer_UnknownAttribute);
                            ms = new System.IO.MemoryStream(receivedResult);
                            newRadioInfo = (RadioInfo?)serializer.Deserialize(ms);

                            if (null != newRadioInfo)
                            {
                                newRadioIdx = newRadioInfo.RadioNr == 1 ? 0 : 1; // convert from one-based user counting to zero-based array index, assumes input is 1 or 2

                                // must've been a well-formed document if we made it here
                                logger?.LogTrace($"ReceiveMessage(): Received deserializable RadioInfo message data for RadioNr {newRadioIdx + 1}:\n,{System.Text.Encoding.ASCII.GetString(receivedResult)}\n");

                                if (null == _radioInfo || null == _radioInfo[newRadioIdx] || _radioInfo[newRadioIdx].isInvalidated)
                                {
                                    logger?.LogInformation($"ReceiveMessage(): will init data for RadioNr {newRadioIdx + 1} from deserialized RadioInfo message data.\n");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logger?.LogDebug($"ReceiveMessage(): Could not deserialize RadioInfo message: {ex.ToString()}, data:\n,{System.Text.Encoding.ASCII.GetString(receivedResult)}\n");
                            continue;
                        }
                        finally
                        {
                            if (null != ms)
                                ms.Dispose();
                        }

                        if (null != newRadioInfo)
                        {
                            // track active radio state only for connected radios, and for "Manual" fake-it radios since those always send false IsConnected status
                            if (newRadioInfo.IsConnected || "Manual" == newRadioInfo.RadioName) // assuming "manual" does not get localized to other languages?
                            {
                                if (_radioInfo[newRadioIdx].isInvalidated || newRadioInfo.EntryWindowHwnd != _radioInfo[newRadioIdx].EntryWindowHwnd)
                                {
                                    logger?.LogInformation($"ReceiveMessage(): got new RadioInfo HWND on radio {newRadioIdx + 1}: {newRadioInfo.EntryWindowHwnd.ToString()} (0x{newRadioInfo.EntryWindowHwnd:X})\n");
                                }

                                if (_radioInfo[newRadioIdx].isInvalidated || newRadioInfo.Freq != _radioInfo[newRadioIdx].Freq)
                                {
                                    logger?.LogInformation($"ReceiveMessage(): got new RadioInfo Freq on radio {newRadioIdx + 1}: {newRadioInfo.Freq.ToString()}\n");
                                }

                                if (newRadioInfo.ActiveRadioNr - 1 != _currentRadioIdx || (_radioInfo[0].isInvalidated && _radioInfo[1].isInvalidated))
                                {
                                    // we're either switching radios, or transitioniong from no-radio to radio-having state
                                    _currentRadioIdx = (ushort)(newRadioInfo.ActiveRadioNr - 1);

                                    // become two-radio aware ONLY if we already had Radio 1, and we are processing a Radio 2 message
                                    // it is not sufficient to receive only a Radio 1 message saying Radio 2 is active, if we don't have any Radio 2 info
                                    if (!_hasSecondRadio && 2 == newRadioInfo.RadioNr)
                                    {
                                        _hasSecondRadio = true;
                                    }
                                    
                                    if (_hasSecondRadio)
                                    {
                                        _client.StateUpdate("n1mm.states.radioConnectionState", $"Radio {newRadioInfo.ActiveRadioNr} of 2");
                                    }
                                    else
                                    {
                                        _client.StateUpdate("n1mm.states.radioConnectionState", $"Radio {newRadioInfo.ActiveRadioNr}");
                                    }
                                    logger?.LogInformation($"ReceiveMessage(): switching Active radio to {newRadioInfo.ActiveRadioNr}\n");
                                }
                                
                                if (!_hasSecondRadio && 2 == newRadioInfo.RadioNr)
                                {
                                    // we now have a new second radio
                                    _hasSecondRadio = true;
                                }

                                // _radioInfo members implement an internal ReaderWriterLockSlim thread safety, many readers but only one writer at a time
                                _radioInfo[newRadioIdx] = newRadioInfo;

                                if (!_radioInfo[0].isInvalidated && !_radioInfo[1].isInvalidated)
                                {
                                    // we have two radios, but the status messages don't arrive in an atomically processable fashion
                                    // quietly sync a few singleton values in N1MM+ that show up in both radios, but do not freshen up the other radio's
                                    // refresh timestamp.  we have a time gap between getting them both and need to ensure no conflicts on singleton data
                                    int otherRadioIdx = 1 & ~newRadioIdx;  // bitwise flip zero to one, or vice versa
                                    _radioInfo[otherRadioIdx].ActiveRadioNr = _radioInfo[newRadioIdx].ActiveRadioNr;
                                    _radioInfo[otherRadioIdx].FocusEntry = _radioInfo[newRadioIdx].FocusEntry;
                                    _radioInfo[otherRadioIdx].FocusRadioNr = _radioInfo[newRadioIdx].FocusRadioNr;
                                }
                            }
                            else
                            {
                                // we have a departing (IsConnected false) radio -- let's update and then invalidate our object that represents it
                                if (0 == newRadioIdx || _radioInfo[0].isInvalidated)
                                {
                                    _radioInfo[0] = newRadioInfo;
                                    // losing Radio 1, or having already lost it, means Radio 2 is also gone and ignoreable
                                    _radioInfo[0].invalidate();
                                    _client.StateUpdate("n1mm.states.radioConnectionState", "Awaiting N1MM+");
                                    if (!_radioInfo[1].isInvalidated)
                                    {
                                        _radioInfo[1].invalidate();
                                        logger?.LogInformation("ReceiveMessage(): Radio 1 has indicated a disconnection, so also invalidating existing Radio 2\n");
                                    }
                                    else
                                    {
                                        logger?.LogInformation("ReceiveMessage(): Radio 1 has indicated a disconnection\n");
                                    }
                                }
                                else
                                {
                                    // getting here means it must be only Radio 2 that's departing
                                    _radioInfo[1] = newRadioInfo;
                                    _radioInfo[1].invalidate();
                                    _client.StateUpdate("n1mm.states.radioConnectionState", "Radio 1");
                                    logger?.LogInformation("ReceiveMessage(): Radio 2 has indicated a disconnection\n");
                                }
                                _hasSecondRadio = false;
                                _currentRadioIdx = 0;
                            }
                        }
                        else
                        {
                            logger?.LogTrace("ReceiveMessage(): ignoring RadioInfo of null type, or having invalid RadioNr or ActiveRadioNr range\n");
                        }
                    }
                    logger?.LogInformation("ReceiveMessage(): _shouldReconnect true, attempt to reinit our RadioInfo broadcast listener\n");
                    if (null != udpClient && null != udpClient.Client)
                    {
                        udpClient.Close();
                    }
                    if (true == _isClosing)
                    {
                        udpClient?.Close();
                        return;
                    }
                    _client.StateUpdate("n1mm.states.radioConnectionState", $"Awaiting N1MM+");
                    _radioInfo[0].invalidate();
                    _radioInfo[1].invalidate();
                    _hasSecondRadio = false;
                    _shouldReconfigureN1mmSockets = false; // finished resetting, will start over on next loop cycle
                }
            });
        }

        private void ProcessSettings(IReadOnlyCollection<TouchPortalSDK.Messages.Models.Setting> settings)
        {
            // Note: settings are stored by TP in the registry under:
            // Computer\HKEY_CURRENT_USER\SOFTWARE\JavaSoft\Prefs\app\core\utilities\/N1/M/M+ /Commands for /Touch /Portal
            if (settings == null)
                return;
            foreach (var s in settings)
            {
                if (null == s.Name || 0 == s.Name.Length)
                {
                    _logger?.LogDebug("ProcessSettings(): got a null or empty setting name");
                    continue;
                }
                if (N1MM_RADIOINFO_BROADCAST_ADDRESS == s.Name)
                {
                    bool isIp = false;
                    if (null != s.Value && 0 < s.Value.Length)
                    {
                        try
                        {
                            System.Net.IPAddress? newIp = null;
                            isIp = System.Net.IPAddress.TryParse(s.Value.Trim(), out newIp);
                            if (true == isIp)
                            {
                                if (null == _n1mmRadioInfoBroadcastIP || !(_n1mmRadioInfoBroadcastIP.Equals(newIp))) {
                                    _n1mmRadioInfoBroadcastIP = newIp;
                                    _shouldReconfigureN1mmSockets = true;
                                    _logger?.LogInformation($"ProcessSettings(): N1MM_RADIOINFO_BROADCAST_ADDRESS updated to {newIp}");
                                }
                                continue;
                            }
                        }
                        finally
                        {
                            // mustn't have been an IP address, or no change from what we had before
                        }
                    }

                    if (!isIp)
                    {
                        // ignore bad setting and use safe default, listen on all interfaces 0.0.0.0 for broadcasts
                        _n1mmRadioInfoBroadcastIP = System.Net.IPAddress.Any;
                        _shouldReconfigureN1mmSockets = true;
                        _logger?.LogWarning($"ProcessSettings(): N1MM_RADIOINFO_BROADCAST_ADDRESS has unusable value '{(null == s.Value ? "null" : s.Value)}', ignoring and using 0.0.0.0");

                        _client.ShowNotification($"{APP_NAME}|Plugin|ProcessSettings|badIP", APP_NAME, 
                            $"Invalid N1MM_RADIOINFO_BROADCAST_ADDRESS setting entered, please re-enter an IP or use 0.0.0.0 for typical solo operator.",
                            new[] {
                                new NotificationOptions { Id = "fixSettings", Title = "Learn more..."}
                        });
                    }
                }
                else if (N1MM_RADIOCMD_LISTENER_ADDRESS == s.Name)
                {
                    bool isIp = false;
                    if (null != s.Value && 0 < s.Value.Length)
                    {
                        try
                        {
                            System.Net.IPAddress? newIp = null;
                            isIp = System.Net.IPAddress.TryParse(s.Value.Trim(), out newIp);
                            if (true == isIp)
                            {
                                if (null == _n1mmRadioCmdListenerIP || !(_n1mmRadioCmdListenerIP.Equals(newIp)))
                                {
                                    _n1mmRadioCmdListenerIP = newIp;
                                    _shouldReconfigureN1mmSockets = true;
                                    _logger?.LogInformation($"ProcessSettings(): N1MM_RADIOCMD_LISTENER_ADDRESS updated to {newIp}");
                                }
                                continue;
                            }
                        }
                        finally
                        {
                            // mustn't have been an IP address, or no change from what we had before
                        }
                    }

                    if (!isIp)
                    {
                        // ignore bad setting and use safe default, send to N1MM+ on loopback interface 127.0.0.1
                        _n1mmRadioCmdListenerIP = System.Net.IPAddress.Loopback;
                        _shouldReconfigureN1mmSockets = true;
                        _logger?.LogWarning($"ProcessSettings(): N1MM_RADIOCMD_LISTENER_ADDRESS has unusable value '{(null == s.Value ? "null" : s.Value)}', ignoring and using 127.0.0.1");

                        _client.ShowNotification($"{APP_NAME}|Plugin|ProcessSettings|badIP", APP_NAME,
                            $"Invalid N1MM_RADIOCMD_LISTENER_ADDRESS setting entered, please re-enter an IP or use 127.0.0.1 for typical solo operator.",
                            new[] {
                                new NotificationOptions { Id = "fixSettings", Title = "Learn more..."}
                        });
                    }
                }
                else if (N1MM_UDP_BROADCAST_PORT == s.Name)
                {
                    ushort port = 0;
                    try
                    {
                        if (null != s.Value && 0 < s.Value.Length && UInt16.TryParse(s.Value, out port)
                            && 0 < port) // ushort tops out at 65535, and Windows is impartial to registered ports 1-1023
                        {
                            if (port != _n1mmRadioInfoBroadcastPort)
                            {
                                _n1mmRadioInfoBroadcastPort = port;
                                _shouldReconfigureN1mmSockets = true;
                                _logger?.LogInformation($"ProcessSettings(): N1MM_UDP_BROADCAST_PORT updated to {port}");
                            }
                            continue;
                        }
                    }
                    finally
                    {
                    }
                    _logger?.LogWarning("ProcessSettings(): N1MM_UDP_BROADCAST_PORT null or NaN, or port outside range 1-65535, ignoring and using 12060");
                    _n1mmRadioInfoBroadcastPort = 12060;

                    _client.ShowNotification($"{APP_NAME}|Plugin|ProcessSettings|badUdpBroadcast", APP_NAME,
                        $"Invalid N1MM_UDP_BROADCAST_PORT entered, please re-enter value between 1-65535 or use default value 12060 for typical solo operator.",
                        new[] {
                                new NotificationOptions { Id = "fixSettings", Title = "Learn more..."}
                        }
                    );
                }
                else if (N1MM_UDP_LISTENER_PORT == s.Name)
                {
                    ushort port = 0;
                    try
                    {
                        if (null != s.Value && 0 < s.Value.Length && UInt16.TryParse(s.Value, out port)
                            && 0 < port) // ushort tops out at 65535, and Windows is impartial to registered ports 1-1023
                        {
                            if (port != _n1mmRadioCmdListenerPort)
                            {
                                _n1mmRadioCmdListenerPort = port;
                                _shouldReconfigureN1mmSockets = true;
                                _logger?.LogInformation($"ProcessSettings(): N1MM_UDP_LISTENER_PORT updated to {port}");
                            }
                            continue;
                        }
                    }
                    finally
                    {
                    }
                    _logger?.LogWarning("ProcessSettings(): N1MM_UDP_LISTENER_PORT null or NaN, or port outside range 1-65535, ignoring and using 13064");
                    _n1mmRadioCmdListenerPort = 13064;

                    _client.ShowNotification($"{APP_NAME}|Plugin|ProcessSettings|badUdpListener", APP_NAME,
                        $"Invalid N1MM_UDP_LISTENER_PORT entered, please re-enter value between 1-65535 or use default value 13064 for typical solo operator.",
                        new[] {
                             new NotificationOptions { Id = "fixSettings", Title = "Learn more..."}
                        }
                    );
                }
                else
                {
                    _logger?.LogInformation($"ProcessSettings(): Ignoring update for unknown setting '{s.Name}'");
                }
            }

            if (true == _shouldReconfigureN1mmSockets)
            {
                _commandSockEndpoint = new System.Net.IPEndPoint(
                    _n1mmRadioCmdListenerIP ?? System.Net.IPAddress.Loopback, 
                    _n1mmRadioCmdListenerPort);
                _logger?.LogInformation("ProcessSettings(): new/changed settings detected that trigger an N1MM+ socket reconfiguration.");
            }
        }

        public void OnInfoEvent(InfoEvent message)
        {
            if (null == message)
                return;
            _logger?.LogDebug($"OnInfoEvent(): got Info event: {message}\n");
            ProcessSettings(message.Settings);
        }

        public void OnListChangedEvent(ListChangeEvent message)
        {
            if (null == message)
                return;
            _logger?.LogDebug($"OnListChangedEvent(): got ListChanged event: {message.Type}, {message.ToString()}\n");
            throw new NotImplementedException();
        }

        public void OnBroadcastEvent(BroadcastEvent message)
        {
            if (null == message)
                return;
            _logger?.LogDebug($"OnBroadcastEvent(): got Broadcast event: {message.Type}, {message.ToString()}\n");
            //throw new NotImplementedException();
        }

        public void OnSettingsEvent(SettingsEvent message)
        {
            // try to process message.values here as a IReadOnlyCollection<TouchPortalSDK.Messages.Models.Setting> object


            if (null == message || null == message.Values)
                return;
            _logger?.LogDebug($"OnSettingsEvent(): got Settings event: {message.Type}, {message.ToString()}\n");
            ProcessSettings(message.Values);
        }

        public void OnActionEvent(ActionEvent message)
        {
            if (null == message)
                return;
            
            RadioInfo radioAtEventTime = _radioInfo[_currentRadioIdx]; // work with whatever we found at call time, so it doesn't change on update
            
            if (true == _isClosing || true == _shouldReconfigureN1mmSockets || null == radioAtEventTime || 
                radioAtEventTime.isInvalidated  || null == _commandSockEndpoint)
            {
                _logger?.LogInformation("OnActionEvent(): No RadioInfo data available yet, or current radio is disconnected.  Waiting for N1MM+ and discarding incoming Action message.\n");
            }
            else if (message.Type == "closePlugin")
            {
                _logger?.LogWarning("OnActionEvent(): received closePlugin action, setting _isClosing.\n");
                _isClosing = true;
            }
            else if (message.Type != "action")
            {
                _logger?.LogDebug("OnActionEvent(): Ignoring unhandled message type.\n");
            }
            else if (null == message.Data || message.Data.Count < 1)
            {
                _logger?.LogDebug("OnActionEvent(): Ignoring action with an empty Data container (no names/values).\n");
            }
            else
            {
                // don't write to log inside a switch case until after the action is dispatched to N1MM+, to help with responsiveness
                switch (message.ActionId)
                {
                    case "n1mm.commands.tp.sendCommandString":
                        // we need to build up an N1MM+ UDP command containing an XML document as follows (no need to pretty print).
                        // RadioNr should be 1 or 2, whichever radio entry window is currently active and designated for transmit activities
                        // FocusEntry should match the FocusEntry hWnd value for the active RadioNr, as sent by N1MM+ RadioInfo broadcasts
                        //   NOTE: EntryWindowHwnd is the real hwnd for the RadioNr sent by RadioInfo, and FocusEntry matches that of ActiveRadioNr
                        // RadioCommand is the string to transmit, with macros, or macro to cause an N1MM+ action to be performed
                        
                        // <?xml version=\"1.0\" encoding=\"utf-8\"?>  <<<<<< declaration not needed by N1MM+, omitting for better perf
                        // <RadioCmd>
                        //     <RadioNr>2</RadioNr>
                        //     <FocusEntry>123456</FocusEntry>
                        //     <RadioCommand>VVV DE {MYCALL}</RadioCommand>
                        // </RadioCmd>

                        // build the full UDP command string in a performant way, without using String.Format or similarly slow approaches.
                        // try to do all our string length counts early, and minimize repeated read of strings from objects
                        int prefixLength = (1 == _currentRadioIdx ? N1MM_RADIOCMD_PREFIX_FOR_RADIO1 : N1MM_RADIOCMD_PREFIX_FOR_RADIO2).Length;
                        int focusEntry = radioAtEventTime.EntryWindowHwnd; // matches _r[_cr].FocusEntry only if _currentRadio is the ActiveRadio
                        int focusEntryLength;
                        // most performant way of counting an Int32 Base10 length with sign, per https://stackoverflow.com/questions/4483886
                        if (focusEntry >= 0)
                        {
                            if (focusEntry < 10) focusEntryLength = 1;
                            else if (focusEntry < 100) focusEntryLength = 2;
                            else if (focusEntry < 1000) focusEntryLength = 3;
                            else if (focusEntry < 10000) focusEntryLength = 4;
                            else if (focusEntry < 100000) focusEntryLength = 5;
                            else if (focusEntry < 1000000) focusEntryLength = 6;
                            else if (focusEntry < 10000000) focusEntryLength = 7;
                            else if (focusEntry < 100000000) focusEntryLength = 8;
                            else if (focusEntry < 1000000000) focusEntryLength = 9;
                            else focusEntryLength = 10;
                        }
                        else
                        {
                            if (focusEntry > -10) focusEntryLength = 2;
                            else if (focusEntry > -100) focusEntryLength = 3;
                            else if (focusEntry > -1000) focusEntryLength = 4;
                            else if (focusEntry > -10000) focusEntryLength = 5;
                            else if (focusEntry > -100000) focusEntryLength = 6;
                            else if (focusEntry > -1000000) focusEntryLength = 7;
                            else if (focusEntry > -10000000) focusEntryLength = 8;
                            else if (focusEntry > -100000000) focusEntryLength = 9;
                            else if (focusEntry > -1000000000) focusEntryLength = 10;
                            else focusEntryLength = 11;
                        }
                        int midfixLength = N1MM_RADIOCMD_MIDFIX.Length;
                        int commandLength = message.Data.First().Value.Length;
                        
                        byte[] command = new byte[
                            prefixLength + focusEntryLength + N1MM_RADIOCMD_MIDFIX.Length + commandLength + N1MM_RADIOCMD_SUFFIX.Length];
                        
                        System.Buffer.BlockCopy(
                            (0 == _currentRadioIdx ? N1MM_RADIOCMD_PREFIX_FOR_RADIO1 : N1MM_RADIOCMD_PREFIX_FOR_RADIO2), 
                            0, command, 0, prefixLength);

                        System.Buffer.BlockCopy(
                            System.Text.Encoding.ASCII.GetBytes(radioAtEventTime.EntryWindowHwnd.ToString()),  // there should be a faster way than this
                            0, command, prefixLength, focusEntryLength);

                        System.Buffer.BlockCopy(
                            N1MM_RADIOCMD_MIDFIX,
                            0, command, prefixLength + focusEntryLength, midfixLength);

                        System.Buffer.BlockCopy(
                            System.Text.Encoding.ASCII.GetBytes(message.Data.First().Value),
                            0, command, prefixLength + focusEntryLength + midfixLength, commandLength);

                        System.Buffer.BlockCopy(
                            N1MM_RADIOCMD_SUFFIX, 0,
                            command, prefixLength + focusEntryLength + midfixLength + commandLength, N1MM_RADIOCMD_SUFFIX.Length);

                        _commandSock.SendTo(command, _commandSockEndpoint);
                        _logger?.LogDebug($"OnActionEvent(): Sending UDP datagram:\n{System.Text.Encoding.ASCII.GetString(command)}\n");
                        break;

                    case "n1mm.commands.tp.sendKeys":
                        // skip checking that all five data are valid, just let the defaults fall through to foregrounding N1MM+ and no keypress
                        if (5 == message.Data.Count) // Ctrl On/Off, Alt On/Off, Shift On/Off, vk from KeypressMappings.cs, allowbg On/Off
                        {
                            bool ctrl = message.Data.GetValueOrDefault("n1mm.commands.tp.sendKeys.press.Data.ctrl", "Off").Equals("On");
                            bool alt = message.Data.GetValueOrDefault("n1mm.commands.tp.sendKeys.press.Data.alt", "Off").Equals("On");
                            bool shift = message.Data.GetValueOrDefault("n1mm.commands.tp.sendKeys.press.Data.shift", "Off").Equals("On");
                            bool allowbg = message.Data.GetValueOrDefault("n1mm.commands.tp.sendKeys.press.Data.allowbg", "Off").Equals("On");

                            // _r[_cr].EntryWindowHwnd matches _r[_cr].FocusEntry only if _currentRadio is the ActiveRadio
                            int targetHwnd = radioAtEventTime.EntryWindowHwnd;

                            // use VK_APPS as a dictionary miss fallthrough (aka the Context Menu key, which is not used by N1MM+ and not present in Entry.tp)
                            InputSimulatorEx.Native.VirtualKeyCode vk =
                                KeypressMappings.VIRTUALKEY.GetValueOrDefault(
                                    message.Data.GetValueOrDefault("n1mm.commands.tp.sendKeys.press.Data.vk", "NO KEY"), 
                                    InputSimulatorEx.Native.VirtualKeyCode.APPS);

                            // foreground the app first, then send the key if it's a valid one
                            SetForegroundWindow(targetHwnd);
                            ShowWindow(targetHwnd, SW_RESTORE);

                            int i = 40;  // wait up to two seconds
                            for (; i > 0; --i )
                            {
                                if (targetHwnd == GetForegroundWindow())
                                    break;  // this break leaves this local for-loop
                                System.Threading.Thread.Sleep(50);
                            }
                            if (0 == i)
                            {
                                _logger?.LogDebug($"OnActionEvent(): Waited too long for Entry window hWnd {targetHwnd} to be foregrounded, so aborting send key action.\n");
                                break; // leave this case block prematurely
                            }

                            if (vk != InputSimulatorEx.Native.VirtualKeyCode.APPS)
                            {
                                // this block may or may not be a good performant approach.
                                // a more succinct block of code might create an IEnumerable<VirtualKeyCode>, loop through adding all three
                                // modifiers based on bool states, and then send the IEnumerable of modifiers along if it's not empty.
                                // Instead, trying here to quickly call exactly what's needed without making, iterating, passing a collection.

                                // assuming most all keyboard layouts have VK_LMENU (Left Alt).  There is no real VK_MENU key, it maybe works but is not real.
                                // http://www.kbdlayout.info/features/virtualkeys/VK_LMENU
                                // https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
                                // https://github.com/radj307/inputsimulator

                                // count modifiers and make exactly the needed IEnumerable, once in its own constructor, for what might be best perf
                                int modifierCount = Convert.ToInt16(ctrl) + Convert.ToInt16(alt) + Convert.ToInt16(shift);

                                if (0 == modifierCount)
                                {
                                    _sim.Keyboard.KeyPress(vk);
                                }
                                else if (1 == modifierCount)
                                {
                                    if (ctrl)
                                    {
                                        _sim.Keyboard.ModifiedKeyStroke(InputSimulatorEx.Native.VirtualKeyCode.CONTROL, vk);
                                    }
                                    else if (alt)
                                    {
                                        _sim.Keyboard.ModifiedKeyStroke(InputSimulatorEx.Native.VirtualKeyCode.LMENU, vk);
                                    }
                                    else // must be shift
                                    {
                                        _sim.Keyboard.ModifiedKeyStroke(InputSimulatorEx.Native.VirtualKeyCode.SHIFT, vk);
                                    }
                                }
                                else if (2 == modifierCount)
                                {
                                    if (!ctrl)
                                    {
                                        _sim.Keyboard.ModifiedKeyStroke(
                                            new[] { InputSimulatorEx.Native.VirtualKeyCode.LMENU, InputSimulatorEx.Native.VirtualKeyCode.SHIFT }, vk);
                                    }
                                    else if (!alt)
                                    {
                                        _sim.Keyboard.ModifiedKeyStroke(
                                            new[] { InputSimulatorEx.Native.VirtualKeyCode.CONTROL, InputSimulatorEx.Native.VirtualKeyCode.SHIFT }, vk);
                                    }
                                    else // must be no shift
                                    {
                                        _sim.Keyboard.ModifiedKeyStroke(
                                            new[] { InputSimulatorEx.Native.VirtualKeyCode.CONTROL, InputSimulatorEx.Native.VirtualKeyCode.LMENU }, vk);
                                    }
                                }
                                else
                                {
                                    // fancy fingerwork, all three modifiers
                                    _sim.Keyboard.ModifiedKeyStroke(
                                        new[] { InputSimulatorEx.Native.VirtualKeyCode.CONTROL, 
                                                InputSimulatorEx.Native.VirtualKeyCode.LMENU,
                                                InputSimulatorEx.Native.VirtualKeyCode.LMENU}, 
                                        vk);
                                }
                            }
                            _logger?.LogDebug($"OnActionEvent(): Foregrounded N1MM+ radio {_currentRadioIdx} at {radioAtEventTime.EntryWindowHwnd} and sent keystrokes.\n");
                        }
                        else
                        {
                            _logger?.LogDebug("OnActionEvent(): Ignoring unsupported or malformed keystroke action.\n");
                        }
                        break;

                    default:
                        _logger?.LogDebug("OnActionEvent(): ignoring unknown Action message.Type\n");
                        break;
                }
            }

            if (null != message.Data && message.Data.Count > 0)
            {
                _logger?.LogDebug($"OnActionEvent(): finished processing Action event: {message.Type}, {message.ActionId}, {message.Data.First().Key}, {message.Data.First().Value}\n");
            }
            else
            {
                _logger?.LogDebug($"OnActionEvent(): finished processing Action event: {message.Type}, {message.ActionId}, <Data container null or empty>\n");
            }
        }

        public void OnNotificationOptionClickedEvent(NotificationOptionClickedEvent message)
        {
            if (null == message)
                return;
            _logger?.LogInformation($"OnNotificationOptionClickedEvent(): got NotificationOptionClicked event: {message.Type}, {message.ToString()}\n");
            
            if (message.NotificationId.StartsWith($"{APP_NAME}|Plugin|ProcessSettings|bad"))
            {
                switch (message.OptionId)
                {
                    //Example for opening a web browser (windows):
                    case "fixSettings":
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            UseShellExecute = true,
                            FileName = "https://github.com/frazierjason/N1mmCommands-TouchPortal#readme"
                        });
                        break;
                }
            }
        }

        public void OnConnecterChangeEvent(ConnectorChangeEvent message)
        {
            if (null == message)
                return;
            _logger?.LogInformation($"OnConnecterChangeEvent(): got ConnectorChange event: {message.Type}, {message.ToString()}\n");
            throw new NotImplementedException();
        }

        public void OnShortConnectorIdNotificationEvent(ShortConnectorIdNotificationEvent connectorInfo)
        {
            string message = connectorInfo?.ToString() ?? "<null recvd>";
            _logger?.LogInformation($"OnShortConnectorIdNotificationEvent(): got ShortConnectorIdNotification event: {message}\n");
            if (null == connectorInfo)
                return;
            throw new NotImplementedException();
        }

        public void OnClosedEvent(string message)
        {
            _logger?.LogInformation($"OnClosedEvent(): calling Exit(0) because we got Closed event: {message ?? "<null recvd>"}\n");
            _shouldReconfigureN1mmSockets = true;
            _commandSock.Close();
            Environment.Exit(0);
        }

        public void OnUnhandledEvent(string jsonMessage)
        {
            _logger?.LogInformation($"OnUnhandledEvent(): got Unhandled event: {jsonMessage ?? "<null recvd>"}\n");
            if (null == jsonMessage)
                return;
            throw new NotImplementedException();
        }
    }

}



