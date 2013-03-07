using System;
using System.Collections.Generic;
using Crosstalk.Core.Models.Relationships;

namespace Crosstalk.Core.Models.Channels
{
    public sealed class ChannelType
    {
        public static class Labels
        {
            public const string Public = "broadcast";
            public const string Private = "private";
            public const string Request = "request";
        }

        private readonly Type _channel;
        private readonly int _value;
        private readonly string _name;
        private static readonly Dictionary<string, ChannelType> Instances = new Dictionary<string, ChannelType>(); 

        public static readonly ChannelType Private = new ChannelType(1, typeof(PrivateChannel), Labels.Private);
        public static readonly ChannelType Public  = new ChannelType(2, typeof(BroadcastChannel), Labels.Public);
        public static readonly ChannelType Request = new ChannelType(3, typeof(RequestChannel), Labels.Request);        

        private ChannelType(int value, Type channel, string name)
        {
            this._name = name;
            this._value = value;
            this._channel = channel;
            Instances[name] = this;
        }

        public Type ToType()
        {
            return this._channel;
        }

        public override string ToString()
        {
            return this._name;
        }

        public static implicit operator ChannelType(string key)
        {
            if (null == key) return null;

            ChannelType type;
            if (Instances.TryGetValue(key, out type))
            {
                return type;
            }
            throw new InvalidCastException("\"" + key + "\" could not be cast to a ChannelType");
        }

        public static implicit operator string(ChannelType type)
        {
            return type.ToString();
        }

        public static implicit operator Type(ChannelType type)
        {
            return type.ToType();
        }
    }
}
