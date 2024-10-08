using Microsoft.Win32;
using Serilog;
using System;

namespace WPF_NhaMayCaoSu.Service.Trial
{
    public class TrialManager
    {
        private const string RegistryKeyPath = @"Software\CaoSuApp";  // Update with actual app name
        private const string StartDateKey = "StartDate";
        private const int TrialDays = 30;

        // Start the trial by setting the current date in the registry
        public void StartTrial()
        {
            using (var key = Registry.CurrentUser.CreateSubKey(RegistryKeyPath))
            {
                if (key.GetValue(StartDateKey) == null)  // Ensure it doesn't reset if already started
                {
                    key.SetValue(StartDateKey, DateTime.Now.ToString());
                }
            }
        }

        // Check if the trial period has expired
        public bool IsTrialExpired()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath))
            {
                if (key != null)
                {
                    var startDateString = key.GetValue(StartDateKey) as string;
                    if (DateTime.TryParse(startDateString, out DateTime startDate))
                    {
                        double totalDays = (DateTime.Now - startDate).TotalDays;
                        Log.Information($"Trial Start Date: {startDate}, Days Passed: {totalDays}, Trial Days: {TrialDays}");
                        return totalDays > TrialDays;
                    }
                }
            }
            return false;
        }


        // Get the number of remaining trial days
        public int GetRemainingTrialDays()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath))
            {
                if (key != null)
                {
                    var startDateString = key.GetValue(StartDateKey) as string;
                    if (DateTime.TryParse(startDateString, out DateTime startDate))
                    {
                        int remainingDays = TrialDays - (int)(DateTime.Now - startDate).TotalDays;
                        return remainingDays > 0 ? remainingDays : 0;
                    }
                }
            }
            return 0;
        }

        // Check if the trial has already started
        public bool HasTrialStarted()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath))
            {
                return key?.GetValue(StartDateKey) != null;
            }
        }
    }
}
