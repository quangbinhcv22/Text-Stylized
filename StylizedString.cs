namespace ThirdParty.NCB.MVVM
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class StylizedString
    {
        public static string Stylized(this string source, StylizedStyle style)
        {
            return StylizedStyleFactory.Get(style).Adapting(source);
        }
    }


    public static class StylizedStyleFactory
    {
        private static readonly Dictionary<StylizedStyle, Type> AdapterTypes = new();
        private static readonly Dictionary<StylizedStyle, IAdapter<string, string>> Adapters = new();

        static StylizedStyleFactory()
        {
            var adapterTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type =>
                type.IsDefined(typeof(StylizedAdapterAttribute)) && !type.IsAbstract);
            var allAdapter = adapterTypes.Select(type =>
                new KeyValuePair<StylizedStyle, Type>(type.GetCustomAttribute<StylizedAdapterAttribute>().Style, type));

            foreach (var adapter in allAdapter)
            {
                AdapterTypes.Add(adapter.Key, adapter.Value);
            }
        }

        public static IAdapter<string, string> Get(StylizedStyle style)
        {
            if (Adapters.ContainsKey(style)) return Adapters[style];

            var newAdapter = (IAdapter<string, string>)Activator.CreateInstance(AdapterTypes[style]);
            Adapters.Add(style, newAdapter);

            return newAdapter;
        }
    }


    [StylizedAdapter(StylizedStyle.Upper)]
    public class UpperCase : IAdapter<string, string>
    {
        public string Adapting(string input)
        {
            return input.ToUpper();
        }
    }

    [StylizedAdapter(StylizedStyle.Lower)]
    public class LowerCase : IAdapter<string, string>
    {
        public string Adapting(string input)
        {
            return input.ToLower();
        }
    }

    public class CamelCase : IAdapter<string, string>
    {
        public string Adapting(string input)
        {
            throw new System.NotImplementedException();
        }
    }


    public interface IAdapter<in TInput, out TOutput>
    {
        public TOutput Adapting(TInput input);
    }


    public enum StylizedStyle
    {
        Lower, //lowercase
        Upper, //UPPERCASE
        Title, //Title Case
        LowerCamel, //lowerCamelCase
        UpperCamel, //UpperCamelCase
        LowerSnake, //lower_snake_case
        UpperSnake, //UPPER_SNAKE_CASE
        LowerCamelSnake, //lower_Camel_Snake_Case
        UpperCamelSnake, //Upper_Camel_Snake_Case
        LowerDash, //lower-dash
        UpperDash, //UPPER-DASH
        Train, //Train-Case
    }


    [AttributeUsage(AttributeTargets.Class)]
    public class StylizedAdapterAttribute : Attribute
    {
        public readonly StylizedStyle Style;

        public StylizedAdapterAttribute(StylizedStyle style)
        {
            Style = style;
        }
    }
}