namespace MassenaenderungPMSv2Gui.Helpers
{
    public class CommandArgsBuilder
    {
        private readonly List<string> _parts = new();

        public CommandArgsBuilder AddFlag(string pre, string name)
        {
            _parts.Add($"{pre}{name}");
            return this;
        }

        public CommandArgsBuilder AddSwitch(string pre, string name)
        {
            _parts.Add($"{pre}{name}");
            return this;
        }

        public CommandArgsBuilder AddOption(string pre, string key, string value)
        {
            // Wert immer quoten
            _parts.Add($"{pre}{key}=\"{value}\"");
            return this;
        }

        public CommandArgsBuilder AddList(string pre, string key, IEnumerable<string> values)
        {
            foreach (var v in values)
                _parts.Add($"{pre}{key}=\"{v}\"");

            return this;
        }

        public CommandArgsBuilder AddRaw(string rawValue)
        {
            _parts.Add(rawValue);
            return this;
        }

        public string Build()
        {
            return string.Join(" ", _parts);
        }
    }
}
