﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MatchMaker.Tool.UI.Properties
{
    /// <summary>
    /// A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources
    {
        /// <summary>
        /// Defines the resourceMan
        /// </summary>
        private static global::System.Resources.ResourceManager resourceMan;

        /// <summary>
        /// Defines the resourceCulture
        /// </summary>
        private static global::System.Globalization.CultureInfo resourceCulture;

        /// <summary>
        /// Initializes a new instance of the <see cref="Resources"/> class.
        /// </summary>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources()
        {
        }

        /// <summary>
        /// Gets the ResourceManager
        /// Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MatchMaker.Tool.UI.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        /// <summary>
        /// Gets or sets the Culture
        /// Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        /// <summary>
        /// Gets the ButtonText_Cancel
        /// Looks up a localized string similar to Cancel.
        /// </summary>
        internal static string ButtonText_Cancel
        {
            get
            {
                return ResourceManager.GetString("ButtonText_Cancel", resourceCulture);
            }
        }

        /// <summary>
        /// Gets the ButtonText_OK
        /// Looks up a localized string similar to OK.
        /// </summary>
        internal static string ButtonText_OK
        {
            get
            {
                return ResourceManager.GetString("ButtonText_OK", resourceCulture);
            }
        }

        /// <summary>
        /// Gets the ButtonText_SelectDestinationFolder
        /// Looks up a localized string similar to Select Destination Folder.
        /// </summary>
        internal static string ButtonText_SelectDestinationFolder
        {
            get
            {
                return ResourceManager.GetString("ButtonText_SelectDestinationFolder", resourceCulture);
            }
        }

        /// <summary>
        /// Gets the ButtonText_SelectSourceFolder
        /// Looks up a localized string similar to Select Source Folder.
        /// </summary>
        internal static string ButtonText_SelectSourceFolder
        {
            get
            {
                return ResourceManager.GetString("ButtonText_SelectSourceFolder", resourceCulture);
            }
        }

        /// <summary>
        /// Gets the LabelText_EnterNumberOfGeneratedTeams
        /// Looks up a localized string similar to Enter number of generated teams:.
        /// </summary>
        internal static string LabelText_EnterNumberOfGeneratedTeams
        {
            get
            {
                return ResourceManager.GetString("LabelText_EnterNumberOfGeneratedTeams", resourceCulture);
            }
        }

        /// <summary>
        /// Gets the LabelText_EnterNumberOfTournamentTeams
        /// Looks up a localized string similar to Enter number of tournament teams:.
        /// </summary>
        internal static string LabelText_EnterNumberOfTournamentTeams
        {
            get
            {
                return ResourceManager.GetString("LabelText_EnterNumberOfTournamentTeams", resourceCulture);
            }
        }
    }
}
