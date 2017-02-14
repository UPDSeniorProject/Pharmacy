using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace VirtualHumanFramework.Core.Messages
{
    // See: http://answers.unity3d.com/questions/363477/c-how-to-setup-a-binary-serialization.html
    // === This is required to guarantee a fixed serialization assembly name, which Unity likes to randomize on each compile
    // Do not change this
    public sealed class VersionDeserializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(typeName))
            {
                Type typeToDeserialize = null;

                assemblyName = Assembly.GetExecutingAssembly().FullName;

                // The following line of code returns the type. 
                typeToDeserialize = Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));

                return typeToDeserialize;
            }

            return null;
        }
    }
}
