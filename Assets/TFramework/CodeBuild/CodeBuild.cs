using System.Text;
using TFramework.ToolBox;
using UnityEngine;

namespace TFramework.CodeBuild
{
    #if UNITY_EDITOR
    public static class CodeBuildSample
    {
        [ToolButton]
        public static void Sample()
        {
            CodeBuild build = new CodeBuild();
            build.Code("// CodeBuildSample")
                .Using("System")
                .Using("System.Collections.Generic")
                .NameSpace("TFramework.CodeBuild.Sample")
                .CodeStart()
                .ClassHead(new ClassBuild
                {
                    ClassName = "SampleClass",
                    IsPublic = true,
                    IsStatic = false,
                    IsPartial = true
                })
                .CodeStart()
                .Code("public float sampleValue = 1;")
                .Code("public bool sampleBool = false;")
                .Code("public void SampleMethod()")
                .CodeStart()
                .Code("Debug.Log(\"SampleMethod\");")
                .CodeEnd()
                .MethodHead(new MethodBuild()
                {
                    IsPublic = true,
                    IsStatic = false,
                    ReturnType = "void",
                    MethodName = "SampleMethod2",
                    Params = "float value1,bool value2"
                })
                .CodeStart()
                .Code("Debug.Log($\"SampleMethod2:{value1} {value2}\");")
                .CodeEnd()
                .CodeEnd()
                .CodeEnd();
            Debug.Log(build.GetCode());
        }
    }
    #endif
    public struct MethodBuild 
    {
        public bool IsPublic;
        public bool IsStatic;
        public bool IsAbstract;
        public string ReturnType;
        public string MethodName;
        public string Params;

        public string BuildHead()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(IsPublic ? "public " : "private ");
            if (IsStatic)
            {
                sb.Append("static ");
            }else if (IsAbstract)
            {
                sb.Append("abstract ");
            }
            sb.Append(string.IsNullOrEmpty(ReturnType) ? "void " : $"{ReturnType} ");
            sb.Append(MethodName);
            sb.Append("(");
            sb.Append(Params);
            sb.Append(")");
            return sb.ToString();
        }
    }
    
    public struct ClassBuild
    {
        public bool IsPublic;
        public bool IsAbstract;
        public bool IsStatic;
        public bool IsSealed;
        public bool IsPartial;
        public string ClassName;
        public string Parent;
        public string[] Extends;
        public string BuildHead()
        {
            StringBuilder sb = new StringBuilder();
            if (IsPublic)
            {
                sb.Append("public ");
            }
            else
            {
                sb.Append("private ");
            }
            if(IsStatic)
            {
                sb.Append("static ");
            }else if (IsSealed)
            {
                sb.Append("sealed ");
            }
            else if (IsAbstract)
            {
                sb.Append("abstract ");
            }
            if (IsPartial)
            {
                sb.Append("partial ");
            }
            sb.Append("class ");
            sb.Append(ClassName);
            if (!string.IsNullOrEmpty(Parent))
            {
                sb.Append(" : ");
                sb.Append(Parent);
                if (Extends is { Length: > 0 })
                {
                    sb.Append(", ");
                    sb.Append(string.Join(", ", Extends));
                }
            }
            else if (Extends is { Length: > 0 })
            {
                sb.Append(" : ");
                sb.Append(string.Join(", ", Extends));
            }

            return sb.ToString();

        }
    }
    public class CodeBuild
    {
        public static string UnityEditorDefine = "UNITY_EDITOR";
        protected StringBuilder _stringBuilder;
        protected int _tabCount = 0;
        public string GetCode() => _stringBuilder.ToString();
        public CodeBuild()
        {
            _stringBuilder = new StringBuilder();
        }
        public CodeBuild(string value)
        {
            _stringBuilder = new StringBuilder(value);
            _stringBuilder.AppendLine();
        }
        public CodeBuild Using(string nameSpace)
        {
            _stringBuilder.AppendLine($"using {nameSpace};");
            return this;
        }
        public CodeBuild DefineIf(string define)
        {
            _stringBuilder.AppendLine($"#if {define}");
            return this;
        }
        public CodeBuild DefineElseIf(string define)
        {
            _stringBuilder.AppendLine($"#elif {define}");
            return this;
        }
        public CodeBuild DefineElse()
        {
            _stringBuilder.AppendLine("#else");
            return this;
        }
        public CodeBuild DefineEnd()
        {
            _stringBuilder.AppendLine("#endif");
            return this;
        }
        
        public CodeBuild NameSpace(string nameSpace)
        {
            _stringBuilder.AppendLine($"namespace {nameSpace}");
            return this;
        }

        public CodeBuild ClassHead(ClassBuild build)
        {
            _stringBuilder.AppendLine($"{Tab}{build.BuildHead()}");
            return this;
        }
        public CodeBuild MethodHead(MethodBuild build)
        {
            _stringBuilder.AppendLine($"{Tab}{build.BuildHead()}");
            return this;
        }
  

        public CodeBuild CodeStart()
        {
            _stringBuilder.AppendLine($"{Tab}{{");
            _tabCount++;
            return this;
        }
        public CodeBuild Code(string code)
        {
            _stringBuilder.AppendLine($"{Tab}{code}");
            return this;
        }
        public CodeBuild CodeEnd()
        {
            _tabCount--;
            _stringBuilder.AppendLine($"{Tab}}}");
            return this;
        }
        public string Tab =>
            new string('\t', _tabCount);

        
    }
}