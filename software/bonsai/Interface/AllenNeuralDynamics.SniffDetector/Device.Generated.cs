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
            { 33, typeof(RawVoltageDispatchRate) }
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
    /// <seealso cref="RawVoltageDispatchRate"/>
    [XmlInclude(typeof(RawVoltage))]
    [XmlInclude(typeof(RawVoltageDispatchRate))]
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
    /// <seealso cref="RawVoltageDispatchRate"/>
    [XmlInclude(typeof(RawVoltage))]
    [XmlInclude(typeof(RawVoltageDispatchRate))]
    [XmlInclude(typeof(TimestampedRawVoltage))]
    [XmlInclude(typeof(TimestampedRawVoltageDispatchRate))]
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
    /// <seealso cref="RawVoltageDispatchRate"/>
    [XmlInclude(typeof(RawVoltage))]
    [XmlInclude(typeof(RawVoltageDispatchRate))]
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
    /// Represents a register that sets the rate at which the RawVoltage event is emitted.
    /// </summary>
    [Description("Sets the rate at which the RawVoltage event is emitted.")]
    public partial class RawVoltageDispatchRate
    {
        /// <summary>
        /// Represents the address of the <see cref="RawVoltageDispatchRate"/> register. This field is constant.
        /// </summary>
        public const int Address = 33;

        /// <summary>
        /// Represents the payload type of the <see cref="RawVoltageDispatchRate"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="RawVoltageDispatchRate"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="RawVoltageDispatchRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="RawVoltageDispatchRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="RawVoltageDispatchRate"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="RawVoltageDispatchRate"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="RawVoltageDispatchRate"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="RawVoltageDispatchRate"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// RawVoltageDispatchRate register.
    /// </summary>
    /// <seealso cref="RawVoltageDispatchRate"/>
    [Description("Filters and selects timestamped messages from the RawVoltageDispatchRate register.")]
    public partial class TimestampedRawVoltageDispatchRate
    {
        /// <summary>
        /// Represents the address of the <see cref="RawVoltageDispatchRate"/> register. This field is constant.
        /// </summary>
        public const int Address = RawVoltageDispatchRate.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="RawVoltageDispatchRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return RawVoltageDispatchRate.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents an operator which creates standard message payloads for the
    /// SniffDetector device.
    /// </summary>
    /// <seealso cref="CreateRawVoltagePayload"/>
    /// <seealso cref="CreateRawVoltageDispatchRatePayload"/>
    [XmlInclude(typeof(CreateRawVoltagePayload))]
    [XmlInclude(typeof(CreateRawVoltageDispatchRatePayload))]
    [XmlInclude(typeof(CreateTimestampedRawVoltagePayload))]
    [XmlInclude(typeof(CreateTimestampedRawVoltageDispatchRatePayload))]
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
    /// that sets the rate at which the RawVoltage event is emitted.
    /// </summary>
    [DisplayName("RawVoltageDispatchRatePayload")]
    [Description("Creates a message payload that sets the rate at which the RawVoltage event is emitted.")]
    public partial class CreateRawVoltageDispatchRatePayload
    {
        /// <summary>
        /// Gets or sets the value that sets the rate at which the RawVoltage event is emitted.
        /// </summary>
        [Description("The value that sets the rate at which the RawVoltage event is emitted.")]
        public ushort RawVoltageDispatchRate { get; set; }

        /// <summary>
        /// Creates a message payload for the RawVoltageDispatchRate register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return RawVoltageDispatchRate;
        }

        /// <summary>
        /// Creates a message that sets the rate at which the RawVoltage event is emitted.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the RawVoltageDispatchRate register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.SniffDetector.RawVoltageDispatchRate.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that sets the rate at which the RawVoltage event is emitted.
    /// </summary>
    [DisplayName("TimestampedRawVoltageDispatchRatePayload")]
    [Description("Creates a timestamped message payload that sets the rate at which the RawVoltage event is emitted.")]
    public partial class CreateTimestampedRawVoltageDispatchRatePayload : CreateRawVoltageDispatchRatePayload
    {
        /// <summary>
        /// Creates a timestamped message that sets the rate at which the RawVoltage event is emitted.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the RawVoltageDispatchRate register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.SniffDetector.RawVoltageDispatchRate.FromPayload(timestamp, messageType, GetPayload());
        }
    }
}
