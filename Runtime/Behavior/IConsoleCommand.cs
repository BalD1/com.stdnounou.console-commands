namespace StdNounou.ConsoleCommands
{
	public interface IConsoleCommand
	{
		public string CommandKey { get; }
        public string CommandArguments { get; }
		public bool Process(string[] args);
    } 
}