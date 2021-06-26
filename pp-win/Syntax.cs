using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pp_win
{
    public record CompilationUnit(List<Definition> Definitions)
    {
    }

    public record Definition(string Name);

    public record FunctionDefinition(
        string Name, 
        List<Parameter> Parameters, 
        TypeReference ReturnType, 
        CompoundStatement Body)
        : Definition(Name);

    public record VariableDefinition(string Name, TypeReference Type) 
        : Definition(Name);

    public record Parameter(string Name, TypeReference Type);

    public record TypeReference(string Name);

    public record Statement;

    public record Assignment(Expression Dst, Expression Src) : Statement;

    
    public record IfStatement(Expression Condition, Statement TrueStatement, Statement FalseStatement) : Statement;

    public record CompoundStatement(List<Statement> Statements) : Statement;

    public record Expression;

    public record Id(string Name) : Expression;

    public record Bin(TokenType Operator, Expression Left, Expression Right) : Expression;

    public record FnApplication(Expression Function, Expression[] Argments) : Expression;
}
