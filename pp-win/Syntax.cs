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

    public abstract record Expression
    {
        public abstract T Accept<T, C>(ExpressionVisitor<T, C> visitor, C context = default);
    }

    public record Id(string Name) : Expression
    {
        public override T Accept<T, C>(ExpressionVisitor<T, C> visitor, C context = default) =>
            visitor.VisitId(this, context);
    }

    public record Bin(TokenType Operator, Expression Left, Expression Right) : Expression
    {
        public override T Accept<T, C>(ExpressionVisitor<T, C> visitor, C context = default) =>
            visitor.VisitBin(this, context);
    }

    public record FnApplication(Expression Function, Expression[] Argments) : Expression
    {
        public override T Accept<T, C>(ExpressionVisitor<T, C> visitor, C context = default) =>
            visitor.VisitApplication(this, context);
    }


    public interface ExpressionVisitor<T, C>
    {
        T VisitApplication(FnApplication fnApplication, C context);
        T VisitBin(Bin bin, C context);
        T VisitId(Id id, C context);
    }

}
