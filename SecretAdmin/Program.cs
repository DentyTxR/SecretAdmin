﻿using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using SecretAdmin.Features.Console;
using SecretAdmin.Features.Program;
using SecretAdmin.Features.Program.Config;
using SecretAdmin.Features.Server;
using SecretAdmin.Features.Server.Commands;
using SecretAdmin.Features.Server.Enums;

namespace SecretAdmin
{
    class Program
    {
        public static Version Version { get; } = new (0, 0, 0,1);
        public static ScpServer Server { get; private set; }
        public static CommandHandler CommandHandler { get; private set; }
        public static ConfigManager ConfigManager { get; private set; }
        public static Logger ProgramLogger { get; private set; }
        
        static void Main(string[] args)
        {
            Console.Title = $"SecretAdmin [v{Version}]";
            AppDomain.CurrentDomain.ProcessExit += OnExit;
            
            Paths.Load();
            
            ConfigManager = new ConfigManager();
            
            if (ProgramIntroduction.FirstTime)
                ProgramIntroduction.ShowIntroduction();
            
            ConfigManager.LoadConfig();
            ProgramLogger = new Logger(Path.Combine(Paths.ProgramLogsFolder, $"{DateTime.Now:MM.dd.yyyy-hh.mm.ss}.log"));
            
            Log.Intro();
            
            Server = new ScpServer(new ServerConfig());
            Server.Start();

            CommandHandler = new CommandHandler();
            InputManager.Start();
        }

        private static void OnExit(object obj, EventArgs ev)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\nExit Detected. Killing game process.");

            if (ConfigManager.SecretAdminConfig.SafeShutdown)
                Server?.Kill();
            
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Everything seems good to go! Bye :)");
        }
    }
}