using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pp_win
{
    class Parser
    {
        private static readonly Token eofToken = new Token(TokenType.EOF);

        private readonly Lexer lexer;
        private Token token;

        public Parser(TextReader rdr)
        {
            this.lexer = new Lexer(rdr);
            this.token = eofToken;
        }

        public CompilationUnit Parse()
        {
            List<Definition> defs = new();
            while (TryDefinition(out var def))
            {
                defs.Add(def);
            }
            return new CompilationUnit(defs);
        }

        private bool TryDefinition(out Definition def)
        {
            def = default;
            if (!TryTypeRef(out TypeReference type)) return false;
            if (!ExpectId(out string id)) return false;
            if (PeekAndDiscard(TokenType.Lparen))
            {
                if (!TryParameterList(out var paramList)) return false;
                if (!Expect(TokenType.Rparen)) return false;
                if (!TryCompoundStatement(out var body)) return false;
                def = new FunctionDefinition(id, paramList, type, (CompoundStatement) body);
            }
            else
            {
                if (!Expect(TokenType.Semi)) return false;
                def = new VariableDefinition(id, type);
            }
            return true;
        }

        private bool TryParameterList(out List<Parameter> parameters)
        {
            parameters = new List<Parameter>();
            while (TryParameter(out Parameter p))
            {
                parameters.Add(p);
            }
            return true;
        }

        private bool TryParameter(out Parameter parameter)
        {
            parameter = default;
            if (!TryTypeRef(out var paramType)) return false;
            if (!ExpectId(out string paramName)) return false;
            parameter = new Parameter(paramName, paramType);
            return true;
        }

        #region Statements 
        
        private bool TryStatement(out Statement stm)
        {
            token = PeekToken();
            switch (token.Type)
            {
            case TokenType.If: return TryIfStatement(out stm);
            case TokenType.Lbrace: return TryCompoundStatement(out stm);
            case TokenType.Id: return TryAssignment(out stm);
            default:
                Error("Expected a statement.");
                stm = default;
                return false;
            }
        }

        private bool TryAssignment(out Statement stm)
        {
            stm = default;
            if (!TryExpression(out Expression dst)) return false;
            if (!Expect(TokenType.Eq)) return false;
            if (!TryExpression(out Expression src)) return false;
            if (!Expect(TokenType.Semi)) return false;
            stm = new Assignment(dst, src);
            return true;
        }

        private bool TryIfStatement(out Statement stm)
        {
            stm = default;
            if (!Expect(TokenType.If)) return false;
            if (!TryExpression(out var condition)) return false;
            if (!TryStatement(out var trueStm)) return false;
            Statement falseStm = default;
            if (PeekAndDiscard(TokenType.Else))
            {
                if (!TryStatement(out falseStm)) return false;
            }
            stm = new IfStatement(condition, trueStm, falseStm);
            return true;
        }

        private bool TryCompoundStatement(out Statement compound)
        {
            compound = default;
            if (!Expect(TokenType.Lbrace)) return false;
            List<Statement> stms = new();
            while (TryStatement(out Statement stm))
            {
                stms.Add(stm);
            }
            compound = new CompoundStatement(stms);
            return true;
        }
        #endregion

        #region Expressions
        private bool TryExpression(out Expression expr)
        {
            if (!TryAndExpression(out expr)) return false;
            while (PeekAndDiscard(TokenType.AndAnd))
            {
                if (!TryAndExpression(out var right)) return false;
                expr = new Bin(TokenType.AndAnd, expr, right);
            }
            return true;
        }

        private bool TryAndExpression(out Expression expr)
        {
            if (!TryAddSubExpression(out expr)) return false;
            while (!PeekAndDiscard(TokenType.And))
            {
                if (TryAddSubExpression(out var right)) return false;
                expr = new Bin(TokenType.And, expr, right);
            }
            return true;
        }

        private bool TryAddSubExpression(out Expression expr)
        {
            if (!TryAtomicExpression(out expr)) return false;
            while (PeekAndDiscard(TokenType.Add))
            {
                if (!TryAtomicExpression(out var right)) return false;
                expr = new Bin(TokenType.Add, expr, right);
            }
            return true;
        }

        private bool TryAtomicExpression(out Expression expr)
        {
            var token = GetToken();
            switch (token.Type)
            {
            case TokenType.Id:
                expr = new Id((string)token.Value);
                return true;
            case TokenType.Lparen:
                if (!TryExpression(out expr)) return false;
                if (!Expect(TokenType.Rparen)) return false;
                return true;
            default:
                Unexpected(token.Type);
                expr = default;
                return false;
            }
        }

        #endregion

        private bool TryTypeRef(out TypeReference type)
        {
            type = default;
            if (!ExpectId(out string id)) return false;
            type = new TypeReference(id);
            return true;
        }

        private bool Expect(TokenType tokenType)
        {
            var token = GetToken();
            return token.Type == tokenType;
        }

        private bool Unexpected(TokenType token)
        {
            return Error($"Expected token type {token}.");
        }

        private bool Error(string message)
        {
            Debug.WriteLine(message);
            return false;
        }

        private bool ExpectId(out string id)
        {
            var token = GetToken();
            if (token.Type == TokenType.Id)
            {
                id = (string)token.Value;
                return true;
            }
            else
            {
                id = default;
                return false;
            }
        }

        private bool PeekAndDiscard(TokenType tokenType)
        {
            var token = PeekToken();
            if (token.Type == tokenType)
            {
                GetToken();
                return true;
            }
            return false;
        }

        private Token GetToken()
        {
            if (this.token.Type == TokenType.EOF)
            {
                return this.lexer.Get();
            }
            else
            {
                Token t = this.token;
                this.token = eofToken;
                return t;
            }
        }

        private Token PeekToken()
        {
            if (this.token.Type == TokenType.EOF)
            {
                this.token = lexer.Get();
            }
            return this.token;
        }
    }

    class Lexer
    {
        private readonly TextReader rdr;
        private readonly StringBuilder sb;

        public Lexer(TextReader rdr)
        {
            this.rdr = rdr;
            this.sb = new StringBuilder();
        }

        public Token Get()
        {
            sb.Clear();
            var state = State.Start;
            for (; ;)
            {
                int c = rdr.Peek();
                char ch = (char)c;
                switch (state)
                {
                case State.Start:
                    switch (c)
                    {
                    case -1: return Tok(TokenType.EOF);
                    case ' ': rdr.Read(); break;
                    case '\t': rdr.Read(); break;
                    case '\n': rdr.Read(); break;
                    case '\r': rdr.Read(); state = State.Cr; break;
                    case '(': rdr.Read(); return Tok(TokenType.Lparen);
                    case ')': rdr.Read(); return Tok(TokenType.Rparen);
                    case '{': rdr.Read(); return Tok(TokenType.Lbrace);
                    case '}': rdr.Read(); return Tok(TokenType.Rbrace);
                    case '=': rdr.Read(); return Tok(TokenType.Eq);
                    case '&': rdr.Read(); state = State.Amp; break;
                    case '+': rdr.Read(); return Tok(TokenType.Add);
                    case ';': rdr.Read(); return Tok(TokenType.Semi);
                    default:
                        if (char.IsLetter(ch))
                        {
                            sb.Append((char)rdr.Read());
                            state = State.Id;
                            break;
                        }
                        throw new NotSupportedException($"Unexpected char '{ch}' (U+{c:X4}.");
                    }
                    break;
                case State.Cr:
                    switch (c)
                    {
                    case -1: return Tok(TokenType.EOF);
                    case '\n': rdr.Read(); state = State.Start; break;
                    default: state = State.Start; break;
                    }
                    break;
                case State.Amp:
                    switch (c)
                    {
                    case -1: return Tok(TokenType.EOF);
                    case '&': rdr.Read(); return Tok(TokenType.AndAnd);
                    default: return Tok(TokenType.And);
                    }
                case State.Id:
                    if (c == -1 || (!char.IsLetterOrDigit(ch) && ch != '_')) 
                    {
                        return Tok(TokenType.Id, sb.ToString());
                    } 
                    else
                    {
                        rdr.Read();
                        sb.Append(ch);
                    }
                    break;
                }
            }
        }

        private Token Tok(TokenType type)
        {
            return new Token(type);
        }

        private Token Tok(TokenType type, object value)
        {
            return new Token(type, value);
        }

        enum State
        {
            Start,
            Id,
            Amp,
            Cr,
        }

        private static readonly Dictionary<string, TokenType> keywords = new  Dictionary<string, TokenType>
        {
            { "if", TokenType.If },
            { "int", TokenType.Int }
        };
    }

    public record Token
    {
        public Token(TokenType type)
        {
            this.Type = type;
            this.Value = null;
        }
        
        public Token(TokenType type, object value)
        {
            this.Type = type;
            this.Value = value;
        }

        public TokenType Type { get; }
        public object Value { get; }
    }

    public enum TokenType
    {
        EOF,
        Lparen,
        Rparen,
        Eq,
        AndAnd,
        And,
        Add,
        If,
        Int,
        Rbrace,
        Lbrace,
        Semi,
        Id,
        Else,
    }
}
