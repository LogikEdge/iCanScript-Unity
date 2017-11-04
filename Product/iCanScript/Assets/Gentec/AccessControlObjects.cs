using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genetec { namespace DoorObjects
{
    public class Door
    {
        public void GrantAccess(bool extendedGrantTime) { }
        public void DenyAccess() { }
        public void Beep() { }
        public void Unlock() { }
        public void Lock() { }
    }

    public class Elevator
    {
        public void ActivateFloors( int[] floors ) { }
    }

    public class ElevatorFloor
    {
    }

    public enum ZoneState
    {
        Active,
        Normal,
        Trouble
    }

    public enum ZoneArmedState
    {
        Armed,
        Disarmed
    }

    public class Zone
    {
        public ZoneState State { get; set; }
        public ZoneArmedState ArmedState { get; set; }
        public bool IsArmed { get; set; }
        public void Arm() { }
        public void Disarm() { }
        public void ActivateMaintenance() { }
        public void DeactivateMaintenance() { }
    }

    public class Area
    {
        public void ActivateLockdown() { }
        public void DeactivateLockdown() { }
    }

    public class Unit
    {
        public bool IsConnected { get; set; }
    }

    public class UnitDevice
    {
        public bool IsConnected { get; set; }
    }

    public abstract class InputState
    {
        public class Normal : InputState { }
        public class Active : InputState { }
        public class Trouble : InputState { }
        public class TroubleCut : Trouble { }
        public class TroubleShort : Trouble { }
    }

    public class DigitalInput : UnitDevice
    {
        public event Action<InputState> Updates; // je sais pas comment tu modélises les events
        public InputState State { get; set; }
        public bool IsActive { get; set; }
        public bool IsNormal { get; set; }
    }

    public class SupervisedInput : DigitalInput
    {
        public bool IsTrouble { get; set; }
    }

    public class Relay
    {
        public bool IsKnownState { get; set; }
        public bool IsActive { get; set; }
    }

    public class Keypad
    {
        public event Action<string> Keypresses;
    }

    public class LcdDisplay
    {
        public void DisplayLcd( string[] text ) { }
    }

    public enum ReaderColor
    {
        Red, Green, Orange, Yellow
    }

    public class Reader
    {
        public string CardRead { get; }
        public void Beep(int durationMs) { }
        public void SetLedColor( ReaderColor c ) { }
    }

    public class AuthenticationService
    {
        public Guid CredentialToCardholder( string cardCode ) { return Guid.Empty; }
    }

    public class CardholderPropertiesService
    {
        public bool HasTrait( Guid cardholder, string traitId ) { return false; }
    }

    public class GroupMembershipService
    {
        public List<Guid> CardholderToGroups(bool expand) { return new List<Guid> { }; } // sans generique je sais pas comment tu veux l'exprimer
    }

    public class AuthorizationService
    {
        public bool HasAccess( /*Guid accessPoint,*/ Guid cardholder, DateTime timestampUtc ) { return false; }
    }

}}

