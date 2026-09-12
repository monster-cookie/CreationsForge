using CreationsForge.Console.Mcp;

if (args.Length > 0 && string.Equals(args[0], "mcp", StringComparison.OrdinalIgnoreCase))
{
    return await McpHostRunner.RunAsync(args[1..]);
}

if (args.Length == 0 || (args.Length == 1 && args[0] is "--help" or "-h" or "help"))
{
    System.Console.WriteLine("Usage: CreationsForge.Console mcp");
    System.Console.WriteLine("Starts the local MCP server on standard input and standard output.");
    return 0;
}

System.Console.Error.WriteLine("Unsupported command. Import and database reset commands have been removed.");
System.Console.Error.WriteLine("Usage: CreationsForge.Console mcp");
return 2;
