﻿using Android.App;
using Android.Runtime;
using AndroidX.AppCompat.App;

namespace CALC_Config
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
            AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightYes;
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
