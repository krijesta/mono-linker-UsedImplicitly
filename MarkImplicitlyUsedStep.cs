using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Linker;
using Mono.Linker.Steps;

namespace ImplicitlyUsed
{
    // This step goes through all of the types, methods and fields of the assemblies looking for the "UsedImplicitly" attribute
    // Anything that has this attribute won't be stripped from the final assembly, this allows for things which are only accessed
    // through reflection to not get stripped (and should be easier to use than specifing in the xml)
    public class MarkImplicitlyUsedStep : BaseStep
    {
        // Checks to see if the type, method or field has the "UsedImplicitly" attribute
        public bool IsUsedImplictly(ICustomAttributeProvider m)
        {
            return m.CustomAttributes.Any(x => x.AttributeType.FullName == "JetBrains.Annotations.UsedImplicitlyAttribute");
        }

        protected override void ProcessAssembly(AssemblyDefinition assembly)
        {
            var types = assembly.MainModule.Types;

            IEnumerable<TypeDefinition> implicitlyUsedTypes = types.Where(IsUsedImplictly);
            IEnumerable<MethodDefinition> implicitlyUsedMethods = types.SelectMany(z => z.Methods).Where(IsUsedImplictly);
            IEnumerable<FieldDefinition> implicitlyUsedFields = types.SelectMany(z => z.Fields).Where(IsUsedImplictly);

            MarkTypes(implicitlyUsedTypes);
            MarkMethods(implicitlyUsedMethods);
            MarkFields(implicitlyUsedFields);
        }

        // Marks all of the types and all of their members so that nothing is stripped
        private void MarkTypes(IEnumerable<TypeDefinition> types)
        {
            foreach (var type in types)
            {
                Annotations.Mark(type);
                Annotations.SetPreserve(type, TypePreserve.All);
            }
        }

        // Marks the method and its declaring type
        private void MarkMethods(IEnumerable<MethodDefinition> methods)
        {
            foreach (var method in methods)
            {
                Annotations.Mark(method);
                Annotations.Mark(method.DeclaringType);
                Annotations.SetAction(method, MethodAction.ForceParse);
            }
        }

        // Marks the fileds and all of their declaring types
        private void MarkFields(IEnumerable<FieldDefinition> fields)
        {
            foreach (var field in fields)
            {
                Annotations.Mark(field);
                Annotations.Mark(field.DeclaringType);
            }
        }
    }
}