using Bonsai;
using Bonsai.Harp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Xml.Serialization;

namespace AllenNeuralDynamics.SniffDetector
{
    /// <summary>
    /// Generates events and processes commands for the SniffDetector device connected
    /// at the specified serial port.
    /// </summary>
    [Combinator(MethodName = nameof(Generate))]
    [WorkflowElementCategory(ElementCategory.Source)]
    [Description("Generates events and processes commands for the SniffDetector device.")]
    public partial class Device : Bonsai.Harp.Device, INamedElement
    {
        /// <summary>
        /// Represents the unique identity class of the <see cref="SniffDetector"/> device.
        /// This field is constant.
        /// </summary>
        public const int WhoAmI = 1401;

        /// <summary>
        /// Initializes a new instance of the <see cref="Device"/> class.
        /// </summary>
        public Device() : base(WhoAmI) { }

        string INamedElement.Name => nameof(SniffDetector);

        /// <summary>
        /// Gets a read-only mapping from address to register type.
        /// </summary>
        public static new IReadOnlyDictionary<int, Type> RegisterMap { get; } = new Dictionary<int, Type>
            (Bonsai.Harp.Device.RegisterMap.ToDictionary(entry => entry.Key, entry => entry.Value))
        {
            { 32, typeof(RawVoltage) },
            { 33, typeof(ErrorState) },
            { 34, typeof(EventEnable) },
            { 35, typeof(EventDispatchFrequency) }
        };
    }

    /// <summary>
    /// Represents an operator that groups the sequence of <see cref="SniffDetector"/>" messages by register type.
    /// </summary>
    [Description("Groups the sequence of SniffDetector messages by register type.")]
    public partial class GroupByRegister : Combinator<HarpMessage, IGroupedObservable<Type, HarpMessage>>
    {
        /// <summary>
        /// Groups an observable sequence of <see cref="SniffDetector"/> messages
        /// by register type.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of observable groups, each of which corresponds to a unique
        /// <see cref="SniffDetector"/> register.
        /// </returns>
        public override IObservable<IGroupedObservable<Type, HarpMessage>> Process(IObservable<HarpMessage> source)
        {
            return source.GroupBy(message => Device.RegisterMap[message.Address]);
        }
    }

    /// <summary>
    /// Represents an operator that filters register-specific messages
    /// reported by the <see cref="SniffDetector"/> device.
    /// </summary>
    /// <seealso cref="RawVoltage"/>
    /// <seealso cref="ErrorState"/>
    /// <seealso cref="EventEnable"/>
    /// <seealso cref="EventDispatchFrequency"/>
    [XmlInclude(typeof(RawVoltage))]
    [XmlInclude(typeof(ErrorState))]
    [XmlInclude(typeof(EventEnable))]
    [XmlInclude(typeof(EventDispatchFrequency))]
    [Description("Filters register-specific messages reported by the SniffDetector device.")]
    public class FilterRegister : FilterRegisterBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterRegister"/> class.
        /// </summary>
        public FilterRegister()
        {
            Register = new RawVoltage();
        }

        string INamedElement.Name
        {
            get => $"{nameof(SniffDetector)}.{GetElementDisplayName(Register)}";
        }
    }

    /// <summary>
    /// Represents an operator which filters and selects specific messages
    /// reported by the SniffDetector device.
    /// </summary>
    /// <seealso cref="RawVoltage"/>
    /// <seealso cref="ErrorState"/>
    /// <seealso cref="EventEnable"/>
    /// <seealso cref="EventDispatchFrequency"/>
    [XmlInclude(typeof(RawVoltage))]
    [XmlInclude(typeof(ErrorState))]
    [XmlInclude(typeof(EventEnable))]
    [XmlInclude(typeof(EventDispatchFrequency))]
    [XmlInclude(typeof(TimestampedRawVoltage))]
    [XmlInclude(typeof(TimestampedErrorState))]
    [XmlInclude(typeof(TimestampedEventEnable))]
    [XmlInclude(typeof(TimestampedEventDispatchFrequency))]
    [Description("Filters and selects specific messages reported by the SniffDetector device.")]
    public partial class Parse : ParseBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Parse"/> class.
        /// </summary>
        public Parse()
        {
            Register = new RawVoltage();
        }

        string INamedElement.Name => $"{nameof(SniffDetector)}.{GetElementDisplayName(Register)}";
    }

    /// <summary>
    /// Represents an operator which formats a sequence of values as specific
    /// SniffDetector register messages.
    /// </summary>
    /// <seealso cref="RawVoltage"/>
    /// <seealso cref="ErrorState"/>
    /// <seealso cref="EventEnable"/>
    /// <seealso cref="EventDispatchFrequency"/>
    [XmlInclude(typeof(RawVoltage))]
    [XmlInclude(typeof(ErrorState))]
    [XmlInclude(typeof(EventEnable))]
    [XmlInclude(typeof(EventDispatchFrequency))]
    [Description("Formats a sequence of values as specific SniffDetector register messages.")]
    public partial class Format : FormatBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Format"/> class.
        /// </summary>
        public Format()
        {
            Register = new RawVoltage();
        }

        string INamedElement.Name => $"{nameof(SniffDetector)}.{GetElementDisplayName(Register)}";
    }

    /// <summary>
    /// Represents a register that emits a periodic event containing the raw voltage read of the thermistor sensor.
    /// </summary>
    [Description("Emits a periodic event containing the raw voltage read of the thermistor sensor.")]
    public partial class RawVoltage
    {
        /// <summary>
        /// Represents the address of the <see cref="RawVoltage"/> register. This field is constant.
        /// </summary>
        public const int Address = 32;

        /// <summary>
        /// Represents the payload type of the <see cref="RawVoltage"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="RawVoltage"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="RawVoltage"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="RawVoltage"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="RawVoltage"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="RawVoltage"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="RawVoltage"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="RawVoltage"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// RawVoltage register.
    /// </summary>
    /// <seealso cref="RawVoltage"/>
    [Description("Filters and selects timestamped messages from the RawVoltage register.")]
    public partial class TimestampedRawVoltage
    {
        /// <summary>
        /// Represents the address of the <see cref="RawVoltage"/> register. This field is constant.
        /// </summary>
        public const int Address = RawVoltage.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="RawVoltage"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return RawVoltage.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that emits an event with error state information.
    /// </summary>
    [Description("Emits an event with error state information.")]
    public partial class ErrorState
    {
        /// <summary>
        /// Represents the address of the <see cref="ErrorState"/> register. This field is constant.
        /// </summary>
        public const int Address = 33;

        /// <summary>
        /// Represents the payload type of the <see cref="ErrorState"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ErrorState"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="ErrorState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static Errors GetPayload(HarpMessage message)
        {
            return (Errors)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ErrorState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Errors> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((Errors)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ErrorState"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ErrorState"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, Errors value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ErrorState"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ErrorState"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, Errors value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ErrorState register.
    /// </summary>
    /// <seealso cref="ErrorState"/>
    [Description("Filters and selects timestamped messages from the ErrorState register.")]
    public partial class TimestampedErrorState
    {
        /// <summary>
        /// Represents the address of the <see cref="ErrorState"/> register. This field is constant.
        /// </summary>
        public const int Address = ErrorState.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ErrorState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Errors> GetPayload(HarpMessage message)
        {
            return ErrorState.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that enables the dispatch of selected events.
    /// </summary>
    [Description("Enables the dispatch of selected events.")]
    public partial class EventEnable
    {
        /// <summary>
        /// Represents the address of the <see cref="EventEnable"/> register. This field is constant.
        /// </summary>
        public const int Address = 34;

        /// <summary>
        /// Represents the payload type of the <see cref="EventEnable"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="EventEnable"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="EventEnable"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static SniffDetectorEvents GetPayload(HarpMessage message)
        {
            return (SniffDetectorEvents)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="EventEnable"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<SniffDetectorEvents> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((SniffDetectorEvents)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="EventEnable"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EventEnable"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, SniffDetectorEvents value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="EventEnable"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EventEnable"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, SniffDetectorEvents value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// EventEnable register.
    /// </summary>
    /// <seealso cref="EventEnable"/>
    [Description("Filters and selects timestamped messages from the EventEnable register.")]
    public partial class TimestampedEventEnable
    {
        /// <summary>
        /// Represents the address of the <see cref="EventEnable"/> register. This field is constant.
        /// </summary>
        public const int Address = EventEnable.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="EventEnable"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<SniffDetectorEvents> GetPayload(HarpMessage message)
        {
            return EventEnable.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that the frequency at which the device will emit periodic events selected under SniffDetectorEvents.
    /// </summary>
    [Description("The frequency at which the device will emit periodic events selected under SniffDetectorEvents.")]
    public partial class EventDispatchFrequency
    {
        /// <summary>
        /// Represents the address of the <see cref="EventDispatchFrequency"/> register. This field is constant.
        /// </summary>
        public const int Address = 35;

        /// <summary>
        /// Represents the payload type of the <see cref="EventDispatchFrequency"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="EventDispatchFrequency"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="EventDispatchFrequency"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="EventDispatchFrequency"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="EventDispatchFrequency"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EventDispatchFrequency"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="EventDispatchFrequency"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EventDispatchFrequency"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// EventDispatchFrequency register.
    /// </summary>
    /// <seealso cref="EventDispatchFrequency"/>
    [Description("Filters and selects timestamped messages from the EventDispatchFrequency register.")]
    public partial class TimestampedEventDispatchFrequency
    {
        /// <summary>
        /// Represents the address of the <see cref="EventDispatchFrequency"/> register. This field is constant.
        /// </summary>
        public const int Address = EventDispatchFrequency.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="EventDispatchFrequency"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return EventDispatchFrequency.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents an operator which creates standard message payloads for the
    /// SniffDetector device.
    /// </summary>
    /// <seealso cref="CreateRawVoltagePayload"/>
    /// <seealso cref="CreateErrorStatePayload"/>
    /// <seealso cref="CreateEventEnablePayload"/>
    /// <seealso cref="CreateEventDispatchFrequencyPayload"/>
    [XmlInclude(typeof(CreateRawVoltagePayload))]
    [XmlInclude(typeof(CreateErrorStatePayload))]
    [XmlInclude(typeof(CreateEventEnablePayload))]
    [XmlInclude(typeof(CreateEventDispatchFrequencyPayload))]
    [XmlInclude(typeof(CreateTimestampedRawVoltagePayload))]
    [XmlInclude(typeof(CreateTimestampedErrorStatePayload))]
    [XmlInclude(typeof(CreateTimestampedEventEnablePayload))]
    [XmlInclude(typeof(CreateTimestampedEventDispatchFrequencyPayload))]
    [Description("Creates standard message payloads for the SniffDetector device.")]
    public partial class CreateMessage : CreateMessageBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateMessage"/> class.
        /// </summary>
        public CreateMessage()
        {
            Payload = new CreateRawVoltagePayload();
        }

        string INamedElement.Name => $"{nameof(SniffDetector)}.{GetElementDisplayName(Payload)}";
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that emits a periodic event containing the raw voltage read of the thermistor sensor.
    /// </summary>
    [DisplayName("RawVoltagePayload")]
    [Description("Creates a message payload that emits a periodic event containing the raw voltage read of the thermistor sensor.")]
    public partial class CreateRawVoltagePayload
    {
        /// <summary>
        /// Gets or sets the value that emits a periodic event containing the raw voltage read of the thermistor sensor.
        /// </summary>
        [Description("The value that emits a periodic event containing the raw voltage read of the thermistor sensor.")]
        public ushort RawVoltage { get; set; }

        /// <summary>
        /// Creates a message payload for the RawVoltage register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return RawVoltage;
        }

        /// <summary>
        /// Creates a message that emits a periodic event containing the raw voltage read of the thermistor sensor.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the RawVoltage register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.SniffDetector.RawVoltage.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that emits a periodic event containing the raw voltage read of the thermistor sensor.
    /// </summary>
    [DisplayName("TimestampedRawVoltagePayload")]
    [Description("Creates a timestamped message payload that emits a periodic event containing the raw voltage read of the thermistor sensor.")]
    public partial class CreateTimestampedRawVoltagePayload : CreateRawVoltagePayload
    {
        /// <summary>
        /// Creates a timestamped message that emits a periodic event containing the raw voltage read of the thermistor sensor.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the RawVoltage register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.SniffDetector.RawVoltage.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that emits an event with error state information.
    /// </summary>
    [DisplayName("ErrorStatePayload")]
    [Description("Creates a message payload that emits an event with error state information.")]
    public partial class CreateErrorStatePayload
    {
        /// <summary>
        /// Gets or sets the value that emits an event with error state information.
        /// </summary>
        [Description("The value that emits an event with error state information.")]
        public Errors ErrorState { get; set; }

        /// <summary>
        /// Creates a message payload for the ErrorState register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public Errors GetPayload()
        {
            return ErrorState;
        }

        /// <summary>
        /// Creates a message that emits an event with error state information.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ErrorState register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.SniffDetector.ErrorState.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that emits an event with error state information.
    /// </summary>
    [DisplayName("TimestampedErrorStatePayload")]
    [Description("Creates a timestamped message payload that emits an event with error state information.")]
    public partial class CreateTimestampedErrorStatePayload : CreateErrorStatePayload
    {
        /// <summary>
        /// Creates a timestamped message that emits an event with error state information.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ErrorState register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.SniffDetector.ErrorState.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that enables the dispatch of selected events.
    /// </summary>
    [DisplayName("EventEnablePayload")]
    [Description("Creates a message payload that enables the dispatch of selected events.")]
    public partial class CreateEventEnablePayload
    {
        /// <summary>
        /// Gets or sets the value that enables the dispatch of selected events.
        /// </summary>
        [Description("The value that enables the dispatch of selected events.")]
        public SniffDetectorEvents EventEnable { get; set; }

        /// <summary>
        /// Creates a message payload for the EventEnable register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public SniffDetectorEvents GetPayload()
        {
            return EventEnable;
        }

        /// <summary>
        /// Creates a message that enables the dispatch of selected events.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the EventEnable register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.SniffDetector.EventEnable.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that enables the dispatch of selected events.
    /// </summary>
    [DisplayName("TimestampedEventEnablePayload")]
    [Description("Creates a timestamped message payload that enables the dispatch of selected events.")]
    public partial class CreateTimestampedEventEnablePayload : CreateEventEnablePayload
    {
        /// <summary>
        /// Creates a timestamped message that enables the dispatch of selected events.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the EventEnable register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.SniffDetector.EventEnable.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that the frequency at which the device will emit periodic events selected under SniffDetectorEvents.
    /// </summary>
    [DisplayName("EventDispatchFrequencyPayload")]
    [Description("Creates a message payload that the frequency at which the device will emit periodic events selected under SniffDetectorEvents.")]
    public partial class CreateEventDispatchFrequencyPayload
    {
        /// <summary>
        /// Gets or sets the value that the frequency at which the device will emit periodic events selected under SniffDetectorEvents.
        /// </summary>
        [Description("The value that the frequency at which the device will emit periodic events selected under SniffDetectorEvents.")]
        public ushort EventDispatchFrequency { get; set; } = 100;

        /// <summary>
        /// Creates a message payload for the EventDispatchFrequency register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return EventDispatchFrequency;
        }

        /// <summary>
        /// Creates a message that the frequency at which the device will emit periodic events selected under SniffDetectorEvents.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the EventDispatchFrequency register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.SniffDetector.EventDispatchFrequency.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that the frequency at which the device will emit periodic events selected under SniffDetectorEvents.
    /// </summary>
    [DisplayName("TimestampedEventDispatchFrequencyPayload")]
    [Description("Creates a timestamped message payload that the frequency at which the device will emit periodic events selected under SniffDetectorEvents.")]
    public partial class CreateTimestampedEventDispatchFrequencyPayload : CreateEventDispatchFrequencyPayload
    {
        /// <summary>
        /// Creates a timestamped message that the frequency at which the device will emit periodic events selected under SniffDetectorEvents.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the EventDispatchFrequency register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.SniffDetector.EventDispatchFrequency.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Available period events.
    /// </summary>
    [Flags]
    public enum SniffDetectorEvents : byte
    {
        None = 0x0,
        RawVoltage = 0x1
    }

    /// <summary>
    /// Available error states in the board
    /// </summary>
    [Flags]
    public enum Errors : byte
    {
        None = 0x0,
        SensorNotDetected = 0x1
    }
}
