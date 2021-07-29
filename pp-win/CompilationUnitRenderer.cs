using Reko.Core.Output;
using System;

namespace pp_win
{
    internal class CompilationUnitRenderer
    {
        private PrettyPrinter pp;

        public CompilationUnitRenderer(PrettyPrinter pp)
        {
            this.pp = pp;
        }

        public void Render(CompilationUnit cu)
        {
            foreach (var def in cu.Definitions)
            {
                pp.BeginGroup();
                switch (def)
                {
                case VariableDefinition vardef: Render(vardef); break;
                case FunctionDefinition fundef: Render(fundef); break;
                }
                pp.EndGroup();
                pp.UnconditionalLineBreak();
                pp.UnconditionalLineBreak();
            }
        }

        private void Render(VariableDefinition vardef)
        {
            Render(vardef.Type);
            pp.Write(" ");
            pp.OptionalLineBreak();
            pp.Write(vardef.Name);
            pp.Write(";");
        }

        private void Render(FunctionDefinition fundef)
        {
            Render(fundef.ReturnType);
            pp.Write(" ");
            pp.OptionalLineBreak();
            pp.Write(fundef.Name);
            pp.Write("(");
            if (fundef.Parameters.Count > 0)
            {
                pp.BeginGroup();
                var sep = "";
                foreach (var parameter in fundef.Parameters)
                {
                    pp.Write(sep);
                    Render(parameter);
                    sep = ", ";
                    pp.ConnectedLineBreak();
                }
                pp.EndGroup();
            }
            throw new NotImplementedException();
        }

        private void Render(Parameter parameter)
        {
            throw new NotImplementedException();
        }

        private void Render(TypeReference type)
        {
            pp.Write(type.Name);
        }
    }
}