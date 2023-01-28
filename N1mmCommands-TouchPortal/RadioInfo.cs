/* This project published at https://github.com/frazierjason/N1mmCommands-TouchPortal
 * under the MIT license.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace N1mmCommands.Touchportal
{
    // represent N1MM+ RadioInfo messages, as documented to be sent every ten seconds or less
    // https://n1mmwp.hamdocs.com/appendices/external-udp-broadcasts/
    // special handling for xsd:boolean noncompliant Pascal-cased True/False, courtesy of
    // https://stackoverflow.com/questions/10511835/xml-serialization-error-on-bool-types
    public class RadioInfo
    {
        // all property accessors in any one instance of this object provide thread safety
        // we allow many reads but self-lock on write, as per example
        // https://learn.microsoft.com/en-us/dotnet/api/system.threading.readerwriterlockslim?view=net-5.0
        [XmlIgnore]
        private ReaderWriterLockSlim cacheLock = new();

        // init on creation (just before XmlSerialization occurs), to track staleness of overall object
        // and callers should use refresh() if a property has to be updated afterwards
        [XmlIgnore]
        private long _refreshTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        [XmlIgnore]
        public long refreshTime
        {
            get
            {
                cacheLock.EnterReadLock();
                try { return _refreshTime; }
                finally { cacheLock.ExitReadLock(); }
            }
        }
        public void refresh()
        {
            cacheLock.EnterWriteLock();
            _refreshTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            cacheLock.ExitWriteLock();
        }

        [XmlIgnore]
        private bool _isInvalidated;
        [XmlIgnore]
        public bool isInvalidated
        {
            get
            {
                cacheLock.EnterReadLock();
                try { return _isInvalidated; }
                finally { cacheLock.ExitReadLock(); }
            }
        }
        public void invalidate()
        {
            _isInvalidated = true;
        }

        [XmlIgnore]
        private string _app = "";
        [XmlElement("app")]
        public string app
        {
            set
            {
                cacheLock.EnterWriteLock();
                _app = value;
                cacheLock.ExitWriteLock();
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _app; }
                finally { cacheLock.ExitReadLock(); }
            }
        }

        [XmlIgnore]
        private string _StationName;
        [XmlElement("StationName")]
        public string StationName
        {
            set
            {
                cacheLock.EnterWriteLock();
                _StationName = value;
                cacheLock.ExitWriteLock();
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _StationName; }
                finally { cacheLock.ExitReadLock(); }
            }
        }

        [XmlIgnore]
        private ushort _RadioNr;
        [XmlElement("RadioNr")]
        public ushort RadioNr
        {
            set
            {
                cacheLock.EnterWriteLock();
                _RadioNr = value;
                cacheLock.ExitWriteLock();
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _RadioNr; }
                finally { cacheLock.ExitReadLock(); }
            }
        }

        [XmlIgnore]
        private string _Freq;
        [XmlElement("Freq")]
        public string Freq
        {
            set
            {
                cacheLock.EnterWriteLock();
                _Freq = value;
                cacheLock.ExitWriteLock();
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _Freq; }
                finally { cacheLock.ExitReadLock(); }
            }
        }

        [XmlIgnore]
        private string _TXFreq;
        [XmlElement("TXFreq")]
        public string TXFreq
        {
            set
            {
                cacheLock.EnterWriteLock();
                _TXFreq = value;
                cacheLock.ExitWriteLock();
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _TXFreq; }
                finally { cacheLock.ExitReadLock(); }
            }
        }

        [XmlIgnore]
        private string _Mode;
        [XmlElement("Mode")]
        public string Mode
        {
            set
            {
                cacheLock.EnterWriteLock();
                _Mode = value;
                cacheLock.ExitWriteLock();
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _Mode; }
                finally { cacheLock.ExitReadLock(); }
            }
        }

        [XmlIgnore]
        private string _OpCall;
        [XmlElement("OpCall")]
        public string OpCall
        {
            set
            {
                cacheLock.EnterWriteLock();
                _OpCall = value;
                cacheLock.ExitWriteLock();
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _OpCall; }
                finally { cacheLock.ExitReadLock(); }
            }
        }

        [XmlIgnore]
        private bool _IsRunning;
        [XmlIgnore]
        public bool IsRunning
        {
            set
            {
                cacheLock.EnterWriteLock();
                _IsRunning = value;
                cacheLock.ExitWriteLock();
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _IsRunning; }
                finally { cacheLock.ExitReadLock(); }
            }
        }
        [XmlElement("IsRunning")]
        public string BackingIsRunning
        {
            set 
            {
                cacheLock.EnterWriteLock();
                try { _IsRunning = bool.Parse(value.ToLowerInvariant()); }
                finally { cacheLock.ExitWriteLock(); }
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _IsRunning.ToString(); }
                finally { cacheLock.ExitReadLock(); }
            }
        }

        [XmlIgnore]
        private int _FocusEntry;
        [XmlElement("FocusEntry")]
        public int FocusEntry
        {
            set
            {
                cacheLock.EnterWriteLock();
                _FocusEntry = value;
                cacheLock.ExitWriteLock();
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _FocusEntry; }
                finally { cacheLock.ExitReadLock(); }
            }
        }

        [XmlIgnore]
        private int _EntryWindowHwnd;
        [XmlElement("EntryWindowHwnd")]
        public int EntryWindowHwnd
        {
            set
            {
                cacheLock.EnterWriteLock();
                _EntryWindowHwnd = value;
                cacheLock.ExitWriteLock();
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _EntryWindowHwnd; }
                finally { cacheLock.ExitReadLock(); }
            }
        }


        private string _Antenna;
        [XmlElement("Antenna")]
        public string Antenna
        {
            set
            {
                cacheLock.EnterWriteLock();
                _Antenna = value;
                cacheLock.ExitWriteLock();
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _Antenna; }
                finally { cacheLock.ExitReadLock(); }
            }
        }

        [XmlIgnore]
        private string _Rotors;
        [XmlElement("Rotors")]
        public string Rotors
        {
            set
            {
                cacheLock.EnterWriteLock();
                _Rotors = value;
                cacheLock.ExitWriteLock();
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _Rotors; }
                finally { cacheLock.ExitReadLock(); }
            }
        }

        [XmlIgnore]
        private ushort _FocusRadioNr;
        [XmlElement("FocusRadioNr")]
        public ushort FocusRadioNr
        {
            set
            {
                cacheLock.EnterWriteLock();
                _FocusRadioNr = value;
                cacheLock.ExitWriteLock();
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _FocusRadioNr; }
                finally { cacheLock.ExitReadLock(); }
            }
        }

        [XmlIgnore]
        private bool _IsStereo;
        [XmlIgnore]
        public bool IsStereo
        {
            set
            {
                cacheLock.EnterWriteLock();
                _IsStereo = value;
                cacheLock.ExitWriteLock();
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _IsStereo; }
                finally { cacheLock.ExitReadLock(); }
            }
        }
        [XmlElement("IsStereo")]
        public string BackingIsStereo
        {
            set
            {
                cacheLock.EnterWriteLock();
                try { _IsStereo = bool.Parse(value.ToLowerInvariant()); }
                finally { cacheLock.ExitWriteLock(); }
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _IsStereo.ToString(); }
                finally { cacheLock.ExitReadLock(); }
            }
        }

        [XmlIgnore]
        private bool _IsSplit;
        [XmlIgnore]
        public bool IsSplit
        {
            set
            {
                cacheLock.EnterWriteLock();
                _IsSplit = value;
                cacheLock.ExitWriteLock();
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _IsSplit; }
                finally { cacheLock.ExitReadLock(); }
            }
        }
        [XmlElement("IsSplit")]
        public string BackingIsSplit
        {
            set
            {
                cacheLock.EnterWriteLock();
                try { _IsSplit = bool.Parse(value.ToLowerInvariant()); }
                finally { cacheLock.ExitWriteLock(); }
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _IsSplit.ToString(); }
                finally { cacheLock.ExitReadLock(); }
            }
        }

        [XmlIgnore]
        private ushort _ActiveRadioNr;
        [XmlElement("ActiveRadioNr")]
        public ushort ActiveRadioNr
        {
            set
            {
                cacheLock.EnterWriteLock();
                _ActiveRadioNr = value;
                cacheLock.ExitWriteLock();
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _ActiveRadioNr; }
                finally { cacheLock.ExitReadLock(); }
            }
        }

        [XmlIgnore]
        private bool _IsTransmitting;
        [XmlIgnore]
        public bool IsTransmitting
        {
            set
            {
                cacheLock.EnterWriteLock();
                _IsTransmitting = value;
                cacheLock.ExitWriteLock();
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _IsTransmitting; }
                finally { cacheLock.ExitReadLock(); }
            }
        }
        [XmlElement("IsTransmitting")]
        public string BackingIsTransmitting
        {
            set
            {
                cacheLock.EnterWriteLock();
                try { _IsTransmitting = bool.Parse(value.ToLowerInvariant()); }
                finally { cacheLock.ExitWriteLock(); }
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _IsTransmitting.ToString(); }
                finally { cacheLock.ExitReadLock(); }
            }
        }

        [XmlIgnore]
        private string _FunctionKeyCaption;
        [XmlElement("FunctionKeyCaption")]
        public string FunctionKeyCaption
        {
            set
            {
                cacheLock.EnterWriteLock();
                _FunctionKeyCaption = value;
                cacheLock.ExitWriteLock();
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _FunctionKeyCaption; }
                finally { cacheLock.ExitReadLock(); }
            }
        }

        [XmlIgnore]
        private string _RadioName;
        [XmlElement("RadioName")]
        public string RadioName
        {
            set
            {
                cacheLock.EnterWriteLock();
                _RadioName = value;
                cacheLock.ExitWriteLock();
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _RadioName; }
                finally { cacheLock.ExitReadLock(); }
            }
        }

        [XmlIgnore]
        private string _AuxAntSelected;
        [XmlElement("AuxAntSelected")]
        public string AuxAntSelected
        {
            set
            {
                cacheLock.EnterWriteLock();
                _AuxAntSelected = value;
                cacheLock.ExitWriteLock();
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _AuxAntSelected; }
                finally { cacheLock.ExitReadLock(); }
            }
        }

        [XmlIgnore]
        private string _AuxAntSelectedName;
        [XmlElement("AuxAntSelectedName")]
        public string AuxAntSelectedName
        {
            set
            {
                cacheLock.EnterWriteLock();
                _AuxAntSelectedName = value;
                cacheLock.ExitWriteLock();
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _AuxAntSelectedName; }
                finally { cacheLock.ExitReadLock(); }
            }
        }

        [XmlIgnore]
        private bool _IsConnected;
        [XmlIgnore]
        public bool IsConnected
        {
            set
            {
                cacheLock.EnterWriteLock();
                _IsConnected = value;
                cacheLock.ExitWriteLock();
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _IsConnected; }
                finally { cacheLock.ExitReadLock(); }
            }
        }
        [XmlElement("IsConnected")]
        public string BackingIsConnected
        {
            set
            {
                cacheLock.EnterWriteLock();
                try { _IsConnected = bool.Parse(value.ToLowerInvariant()); }
                finally { cacheLock.ExitWriteLock(); }
            }
            get
            {
                cacheLock.EnterReadLock();
                try { return _IsConnected.ToString(); }
                finally { cacheLock.ExitReadLock(); }
            }
        }
    }

/*
<? xml version="1.0" encoding="utf-8"?>
<RadioInfo>
        <app>N1MM</app>
        <StationName>WHAM</StationName>
        <RadioNr>1</RadioNr>
        <Freq>1405946</Freq>
        <TXFreq>1405946</TXFreq>
        <Mode>USB</Mode>
        <OpCall>W7DM</OpCall>
        <IsRunning>False</IsRunning>
        <FocusEntry>135214</FocusEntry>
        <EntryWindowHwnd>135214</EntryWindowHwnd>
        <Antenna>-1</Antenna>
        <Rotors>-1</Rotors>
        <FocusRadioNr>1</FocusRadioNr>
        <IsStereo>False</IsStereo>
        <IsSplit>False</IsSplit>
        <ActiveRadioNr>1</ActiveRadioNr>
        <IsTransmitting>False</IsTransmitting>
        <FunctionKeyCaption></FunctionKeyCaption>
        <RadioName>Manual</RadioName>
        <AuxAntSelected>-1</AuxAntSelected>
        <AuxAntSelectedName></AuxAntSelectedName>
        <IsConnected>False</IsConnected>
</RadioInfo>
*/
}
