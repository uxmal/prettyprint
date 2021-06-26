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
            throw new NotImplementedException();
        }

        private void Render(TypeReference type)
        {
            pp.Write(type.Name);
        }
    }
}