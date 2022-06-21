using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace BaseFramework
{
    public class ExceptionHandlerManager : Singleton<ExceptionHandlerManager>
    {
        public const string LogFileName = "CustomExceptionHandler.txt";

        public override void Init()
        {
            base.Init();

            Application.logMessageReceived += Handler;
        }

        public override void Release()
        {
            base.Release();

            Application.logMessageReceived -= Handler;
        }

        private void Handler(string condition, string stackTrace, LogType type)
        {
            string time = DateTime.Now.ToString("yyyy_MM-dd HH_mm_ss");
            string filePath = Path.Combine(Application.persistentDataPath, LogFileName);

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }

            StringBuilder result = new StringBuilder();

            result.Append($"[time]:{time}\r\n");
            result.Append($"[type]:{type}\r\n");
            result.Append($"[condition]:{condition}\r\n");
            result.Append($"[stackTrace]:{stackTrace}\r\n");

            File.AppendAllText(filePath, result.ToString());
        }
    }
}
