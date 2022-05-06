﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MultiPlug.Ext.SerialPort.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MultiPlug.Ext.SerialPort.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap SerialPort {
            get {
                object obj = ResourceManager.GetObject("SerialPort", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to @model MultiPlug.Base.Http.EdgeApp
        ///
        ///&lt;section class=&quot;row-fluid&quot;&gt;
        ///
        ///    &lt;div class=&quot;row-fluid&quot;&gt;
        ///        &lt;div class=&quot;box&quot;&gt;
        ///            &lt;div class=&quot;span2&quot;&gt;
        ///                &lt;a style=&quot;line-height: 52px;&quot; href=&quot;@Raw(Model.Context.Paths.Home)&quot;&gt;&lt;img alt=&quot;Serial Port Icon&quot; src=&quot;@Raw(Model.Context.Paths.Assets)images/serialport.png&quot;&gt;&lt;/a&gt;
        ///            &lt;/div&gt;
        ///            &lt;div class=&quot;span8&quot;&gt;
        ///                &lt;p style=&quot;font-size:26px; line-height: 54px; text-align: center; margin: 0px;&quot;&gt;About&lt;/p&gt;
        ///            &lt;/div [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SettingsAbout {
            get {
                return ResourceManager.GetString("SettingsAbout", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to @model MultiPlug.Base.Http.EdgeApp
        ///
        ///@functions{
        ///
        ///    string IsSerialPortOpen(bool theValue)
        ///    {
        ///        return (theValue) ? &quot;&lt;i class=\&quot;icon-ok\&quot;&gt;&lt;/i&gt;&quot; : &quot;&lt;i class=\&quot;icon-remove\&quot;&gt;&lt;/i&gt;&quot;;
        ///    }
        ///}
        ///&lt;form action=&quot;&quot; method=&quot;post&quot; accept-charset=&quot;utf-8&quot; enctype=&quot;application/x-www-form-urlencoded&quot; autocomplete=&quot;off&quot;&gt;
        ///    &lt;section class=&quot;row-fluid&quot;&gt;
        ///
        ///        &lt;div class=&quot;row-fluid&quot;&gt;
        ///            &lt;div class=&quot;box&quot;&gt;
        ///                &lt;div class=&quot;span2&quot;&gt;
        ///                    &lt;span style=&quot;display: inline-blo [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SettingsHome {
            get {
                return ResourceManager.GetString("SettingsHome", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to @model MultiPlug.Base.Http.EdgeApp
        ///
        ///@functions
        ///{
        ///    string isChecked(bool theValue)
        ///    {
        ///        return (theValue) ? &quot;checked&quot; : &quot;&quot;;
        ///    }
        ///
        ///    string isChecked(int theValue1, int theValue2)
        ///    {
        ///        return (theValue1 == theValue2) ? &quot;checked&quot; : &quot;&quot;;
        ///    }
        ///
        ///    string isSelected(string theValue1, string theValue2)
        ///    {
        ///        return (theValue1 == theValue2) ? &quot;selected&quot; : &quot;&quot;;
        ///    }
        ///
        ///    string isSelected(int theValue1, int theValue2)
        ///    {
        ///        return (theValue1 == theValue2) [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SettingsSerialPort {
            get {
                return ResourceManager.GetString("SettingsSerialPort", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to @model MultiPlug.Base.Http.EdgeApp
        ///@functions {
        ///    public string NavLocationIsHome()
        ///    {
        ///        return Model.Context.Paths.Current == Model.Context.Paths.Home ? &quot;active&quot; : string.Empty;
        ///    }
        ///
        ///    public string NavLocationIsSerialPort()
        ///    {
        ///        return Model.Context.Paths.Current == Model.Context.Paths.Home + &quot;serialport/&quot; ? &quot;active&quot; : string.Empty;
        ///    }
        ///
        ///    public string NavLocationIsStatus()
        ///    {
        ///        return Model.Context.Paths.Current == Model.Context.Paths.Home + &quot;status/&quot; ? [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SettingsSerialPortNavigation {
            get {
                return ResourceManager.GetString("SettingsSerialPortNavigation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to @model MultiPlug.Base.Http.EdgeApp
        ///
        ///@functions
        ///{
        ///    string isCurrentLoggingLevel(int theCurrentLevel, int theLevel)
        ///    {
        ///        return (theCurrentLevel == theLevel) ? &quot;selected&quot; : &quot;&quot;;
        ///    }
        ///}
        ///
        ///&lt;section class=&quot;row-fluid&quot;&gt;
        ///
        ///    &lt;div class=&quot;row-fluid&quot;&gt;
        ///        &lt;div class=&quot;box&quot;&gt;
        ///            &lt;div class=&quot;span2&quot;&gt;
        ///                &lt;a style=&quot;line-height: 52px;&quot; href=&quot;@Raw(Model.Context.Paths.Home)&quot;&gt;&lt;img alt=&quot;Serial Port Icon&quot; src=&quot;@Raw(Model.Context.Paths.Assets)images/serialport.png&quot;&gt;&lt;/a&gt;
        ///         [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SettingsStatus {
            get {
                return ResourceManager.GetString("SettingsStatus", resourceCulture);
            }
        }
    }
}
