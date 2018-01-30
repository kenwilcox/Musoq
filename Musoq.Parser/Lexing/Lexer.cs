﻿using System.Text.RegularExpressions;
using FQL.Parser.Nodes;
using FQL.Parser.Tokens;

namespace FQL.Parser.Lexing
{
    public class Lexer : LexerBase<Token>
    {
        private readonly bool _skipWhiteSpaces;

        /// <summary>
        ///     Initialize instance.
        /// </summary>
        /// <param name="input">The query.</param>
        /// <param name="skipWhiteSpaces">Should skip whitespaces?</param>
        public Lexer(string input, bool skipWhiteSpaces) :
            base(input, new NoneToken(), DefinitionSets.General)
        {
            _skipWhiteSpaces = skipWhiteSpaces;
        }

        /// <summary>
        ///     Resolve the statement type.
        /// </summary>
        /// <param name="tokenText">Text that match some definition</param>
        /// <param name="matchedDefinition">Definition that text matched.</param>
        /// <returns>Statement type.</returns>
        private TokenType GetTokenCandidate(string tokenText, TokenDefinition matchedDefinition)
        {
            switch (tokenText.ToLowerInvariant())
            {
                case AndToken.TokenText:
                    return TokenType.And;
                case CommaToken.TokenText:
                    return TokenType.Comma;
                case DiffToken.TokenText:
                    return TokenType.Diff;
                case GreaterToken.TokenText:
                    return TokenType.Greater;
                case GreaterEqualToken.TokenText:
                    return TokenType.GreaterEqual;
                case HyphenToken.TokenText:
                    return TokenType.Hyphen;
                case LeftParenthesisToken.TokenText:
                    return TokenType.LeftParenthesis;
                case RightParenthesisToken.TokenText:
                    return TokenType.RightParenthesis;
                case LessToken.TokenText:
                    return TokenType.Less;
                case LessEqualToken.TokenText:
                    return TokenType.LessEqual;
                case ModuloToken.TokenText:
                    return TokenType.Mod;
                case NotToken.TokenText:
                    return TokenType.Not;
                case OrToken.TokenText:
                    return TokenType.Or;
                case PlusToken.TokenText:
                    return TokenType.Plus;
                case StarToken.TokenText:
                    return TokenType.Star;
                case WhereToken.TokenText:
                    return TokenType.Where;
                case WhiteSpaceToken.TokenText:
                    return TokenType.WhiteSpace;
                case SelectToken.TokenText:
                    return TokenType.Select;
                case FromToken.TokenText:
                    return TokenType.From;
                case EqualityToken.TokenText:
                    return TokenType.Equality;
                case LikeToken.TokenText:
                    return TokenType.Like;
                case AsToken.TokenText:
                    return TokenType.As;
                case SetOperatorToken.ExceptOperatorText:
                    return TokenType.Except;
                case SetOperatorToken.IntersectOperatorText:
                    return TokenType.Intersect;
                case SetOperatorToken.UnionOperatorText:
                    return TokenType.Union;
                case DotToken.TokenText:
                    return TokenType.Dot;
                case HavingToken.TokenText:
                    return TokenType.Having;
            }

            if (string.IsNullOrWhiteSpace(tokenText))
                return TokenType.WhiteSpace;

            if (int.TryParse(tokenText, out var _) && !tokenText.Contains(" "))
                return TokenType.Integer;
            if (matchedDefinition.Regex.ToString() == TokenRegexDefinition.KKeyObjectAccess)
                return TokenType.KeyAccess;
            if (matchedDefinition.Regex.ToString() == TokenRegexDefinition.KNumericArrayAccess)
                return TokenType.NumericAccess;
            if (matchedDefinition.Regex.ToString() == TokenRegexDefinition.KDecimal)
                return TokenType.Decimal;
            if (matchedDefinition.Regex.ToString() == TokenRegexDefinition.KGroupBy)
                return TokenType.GroupBy;
            if (matchedDefinition.Regex.ToString() == TokenRegexDefinition.KUnionAll)
                return TokenType.UnionAll;
            if (matchedDefinition.Regex.ToString() == TokenRegexDefinition.Function)
                return TokenType.Function;
            var last = Current();
            if (matchedDefinition.Regex.ToString() == TokenRegexDefinition.KColumn && last != null && last.TokenType == TokenType.Dot)
                return TokenType.Property;
            if (matchedDefinition.Regex.ToString() == TokenRegexDefinition.KColumn)
                return TokenType.Column;
            if (matchedDefinition.Regex.ToString() == TokenRegexDefinition.KHFrom)
                return TokenType.Word;

            return TokenType.Word;
        }

        /// <summary>
        ///     The token regexes set.
        /// </summary>
        private static class TokenRegexDefinition
        {
            private const string Keyword = @"(?<=[\s]{{1,}}|^){0}(?=[\s]{{1,}}|$)";
            public const string Function = @"[a-zA-Z_-]{1,}[a-zA-Z1-9_-]{1,}[\d]*(?=[\(])";

            public static readonly string KAnd = string.Format(Keyword, AndToken.TokenText);
            public static readonly string KComma = CommaToken.TokenText;
            public static readonly string KDiff = DiffToken.TokenText;
            public static readonly string KfSlashToken = string.Format(Keyword, FSlashToken.TokenText);
            public static readonly string KGreater = string.Format(Keyword, GreaterToken.TokenText);
            public static readonly string KGreaterEqual = string.Format(Keyword, GreaterEqualToken.TokenText);
            public static readonly string KHyphen = $@"\{HyphenToken.TokenText}";
            public static readonly string KLeftParenthesis = $@"\{LeftParenthesisToken.TokenText}";
            public static readonly string KLess = string.Format(Keyword, LessToken.TokenText);
            public static readonly string KLessEqual = string.Format(Keyword, LessEqualToken.TokenText);
            public static readonly string KModulo = string.Format(Keyword, ModuloToken.TokenText);
            public static readonly string KNot = string.Format(Keyword, NotToken.TokenText);
            public static readonly string KOr = string.Format(Keyword, OrToken.TokenText);
            public static readonly string KPlus = $@"\{PlusToken.TokenText}";
            public static readonly string KRightParenthesis = $@"\{RightParenthesisToken.TokenText}";
            public static readonly string KStar = string.Format(Keyword, $@"\{StarToken.TokenText}");
            public static readonly string KWhere = string.Format(Keyword, WhereToken.TokenText);
            public static readonly string KWhiteSpace = @"[\s]{1,}";
            public static readonly string KWordBracketed = @"'(.*?[^\\])'";
            public static readonly string KEqual = string.Format(Keyword, EqualityToken.TokenText);
            public static readonly string KSelect = string.Format(Keyword, SelectToken.TokenText);
            public static readonly string KFrom = string.Format(Keyword, FromToken.TokenText);
            public static readonly string KColumn = @"[\w*?_]{1,}";
            public static readonly string KHFrom = @"#[\w*?_]{1,}";
            public static readonly string KLike = string.Format(Keyword, LikeToken.TokenText);
            public static readonly string KAs = string.Format(Keyword, AsToken.TokenText);
            public static readonly string KUnion = string.Format(Keyword, SetOperatorToken.UnionOperatorText);
            public static readonly string KDot = "\\.";
            public static readonly string KIntersect = string.Format(Keyword, SetOperatorToken.IntersectOperatorText);
            public static readonly string KExcept = string.Format(Keyword, SetOperatorToken.ExceptOperatorText);
            public static readonly string KCompareWith = @"(?<=[\s]{1,}|^)compare[\s]{1,}with(?=[\s]{1,}|$)";
            public static readonly string KUnionAll = @"(?<=[\s]{1,}|^)union[\s]{1,}all(?=[\s]{1,}|$)";
            public static readonly string KGroupBy = @"(?<=[\s]{1,}|^)group[\s]{1,}by(?=[\s]{1,}|$)";
            public static readonly string KHaving = string.Format(Keyword, HavingToken.TokenText);
            public static readonly string KDecimal = @"([0-9]+(\.[0-9]{1,})?)d";
            public static readonly string KNumericArrayAccess = "([\\w*?_]{1,})\\[([0-9]{1,})\\]";
            public static readonly string KKeyObjectAccess = "([\\w*?_]{1,})\\[([a-zA-Z0-9]{1,})\\]";
        }

        /// <summary>
        ///     The token definitions set.
        /// </summary>
        private static class DefinitionSets
        {
            /// <summary>
            ///     All supported by language keyword.
            /// </summary>
            public static TokenDefinition[] General => new[]
            {
                new TokenDefinition(TokenRegexDefinition.KDecimal),
                new TokenDefinition(TokenRegexDefinition.KAs),
                new TokenDefinition(TokenRegexDefinition.KAnd),
                new TokenDefinition(TokenRegexDefinition.KComma),
                new TokenDefinition(TokenRegexDefinition.KDiff),
                new TokenDefinition(TokenRegexDefinition.KfSlashToken),
                new TokenDefinition(TokenRegexDefinition.KGreater),
                new TokenDefinition(TokenRegexDefinition.KGreaterEqual),
                new TokenDefinition(TokenRegexDefinition.KHyphen),
                new TokenDefinition(TokenRegexDefinition.KLeftParenthesis),
                new TokenDefinition(TokenRegexDefinition.KRightParenthesis),
                new TokenDefinition(TokenRegexDefinition.KLess),
                new TokenDefinition(TokenRegexDefinition.KLessEqual),
                new TokenDefinition(TokenRegexDefinition.KEqual),
                new TokenDefinition(TokenRegexDefinition.KModulo),
                new TokenDefinition(TokenRegexDefinition.KNot),
                new TokenDefinition(TokenRegexDefinition.KOr),
                new TokenDefinition(TokenRegexDefinition.KPlus),
                new TokenDefinition(TokenRegexDefinition.KStar),
                new TokenDefinition(TokenRegexDefinition.KWhere),
                new TokenDefinition(TokenRegexDefinition.KWhiteSpace),
                new TokenDefinition(TokenRegexDefinition.KUnionAll),
                new TokenDefinition(TokenRegexDefinition.Function),
                new TokenDefinition(TokenRegexDefinition.KWordBracketed, RegexOptions.ECMAScript),
                new TokenDefinition(TokenRegexDefinition.KSelect),
                new TokenDefinition(TokenRegexDefinition.KFrom),
                new TokenDefinition(TokenRegexDefinition.KUnion),
                new TokenDefinition(TokenRegexDefinition.KExcept),
                new TokenDefinition(TokenRegexDefinition.KIntersect),
                new TokenDefinition(TokenRegexDefinition.KGroupBy),
                new TokenDefinition(TokenRegexDefinition.KHaving),
                new TokenDefinition(TokenRegexDefinition.KNumericArrayAccess),
                new TokenDefinition(TokenRegexDefinition.KKeyObjectAccess),
                new TokenDefinition(TokenRegexDefinition.KColumn),
                new TokenDefinition(TokenRegexDefinition.KHFrom),
                new TokenDefinition(TokenRegexDefinition.KLike),
                new TokenDefinition(TokenRegexDefinition.KDot)
            };
        }

        #region Overrides of LexerBase<Token>

        /// <summary>
        ///     Gets the next token from tokens stream.
        /// </summary>
        /// <returns>The token.</returns>
        public override Token Next()
        {
            var token = base.Next();
            while (_skipWhiteSpaces && token.TokenType == TokenType.WhiteSpace)
                token = base.Next();
            return token;
        }

        /// <summary>
        ///     Gets EndOfFile token.
        /// </summary>
        /// <returns>End of file token.</returns>
        protected override Token GetEndOfFileToken()
        {
            return new EndOfFileToken(new TextSpan(Input.Length, 0));
        }

        /// <summary>
        ///     Gets the token.
        /// </summary>
        /// <param name="matchedDefinition">The definition of token type that fits requirements.</param>
        /// <param name="match">The match.</param>
        /// <returns>The token.</returns>
        protected override Token GetToken(TokenDefinition matchedDefinition, Match match)
        {
            var tokenText = match.Value;
            var token = GetTokenCandidate(tokenText, matchedDefinition);

            switch (token)
            {
                case TokenType.And:
                    return new AndToken(new TextSpan(Position, tokenText.Length));
                case TokenType.Comma:
                    return new CommaToken(new TextSpan(Position, tokenText.Length));
                case TokenType.Diff:
                    return new DiffToken(new TextSpan(Position, tokenText.Length));
                case TokenType.Equality:
                    return new EqualityToken(new TextSpan(Position, tokenText.Length));
                case TokenType.FSlash:
                    return new FSlashToken(new TextSpan(Position, tokenText.Length));
                case TokenType.Function:
                    return new FunctionToken(tokenText, new TextSpan(Position, tokenText.Length));
                case TokenType.Greater:
                    return new GreaterToken(new TextSpan(Position, tokenText.Length));
                case TokenType.GreaterEqual:
                    return new GreaterEqualToken(new TextSpan(Position, tokenText.Length));
                case TokenType.Hyphen:
                    return new HyphenToken(new TextSpan(Position, tokenText.Length));
                case TokenType.LeftParenthesis:
                    return new LeftParenthesisToken(new TextSpan(Position, tokenText.Length));
                case TokenType.Less:
                    return new LessToken(new TextSpan(Position, tokenText.Length));
                case TokenType.LessEqual:
                    return new LessEqualToken(new TextSpan(Position, tokenText.Length));
                case TokenType.Mod:
                    return new ModuloToken(new TextSpan(Position, tokenText.Length));
                case TokenType.Not:
                    return new NotToken(new TextSpan(Position, tokenText.Length));
                case TokenType.Decimal:
                    return new DecimalToken(tokenText.TrimEnd('d'), new TextSpan(Position, tokenText.Length));
                case TokenType.Integer:
                    return new IntegerToken(tokenText, new TextSpan(Position, tokenText.Length));
                case TokenType.Or:
                    return new OrToken(new TextSpan(Position, tokenText.Length));
                case TokenType.Plus:
                    return new PlusToken(new TextSpan(Position, tokenText.Length));
                case TokenType.RightParenthesis:
                    return new RightParenthesisToken(new TextSpan(Position, tokenText.Length));
                case TokenType.Star:
                    return new StarToken(new TextSpan(Position, tokenText.Length));
                case TokenType.Where:
                    return new WhereToken(new TextSpan(Position, tokenText.Length));
                case TokenType.WhiteSpace:
                    return new WhiteSpaceToken(new TextSpan(Position, tokenText.Length));
                case TokenType.From:
                    return new FromToken(new TextSpan(Position, tokenText.Length));
                case TokenType.Select:
                    return new SelectToken(new TextSpan(Position, tokenText.Length));
                case TokenType.Column:
                    return new ColumnToken(tokenText, new TextSpan(Position, tokenText.Length));
                case TokenType.Property:
                    return new AccessPropertyToken(tokenText, new TextSpan(Position, tokenText.Length));
                case TokenType.Like:
                    return new LikeToken(new TextSpan(Position, tokenText.Length));
                case TokenType.As:
                    return new AsToken(new TextSpan(Position, tokenText.Length));
                case TokenType.Except:
                    return new ExceptToken(new TextSpan(Position, tokenText.Length));
                case TokenType.Union:
                    return new UnionToken(new TextSpan(Position, tokenText.Length));
                case TokenType.Intersect:
                    return new IntersectToken(new TextSpan(Position, tokenText.Length));
                case TokenType.UnionAll:
                    return new UnionAllToken(new TextSpan(Position, tokenText.Length));
                case TokenType.Dot:
                    return new DotToken(new TextSpan(Position, tokenText.Length));
                case TokenType.GroupBy:
                    return new GroupByToken(new TextSpan(Position, tokenText.Length));
                case TokenType.Having:
                    return new HavingToken(new TextSpan(Position, tokenText.Length));
                case TokenType.NumericAccess:
                    match = matchedDefinition.Regex.Match(tokenText);
                    return new NumericAccessToken(match.Groups[1].Value, match.Groups[2].Value, new TextSpan(Position, tokenText.Length));
                case TokenType.KeyAccess:
                    match = matchedDefinition.Regex.Match(tokenText);
                    return new KeyAccessToken(match.Groups[1].Value, match.Groups[2].Value, new TextSpan(Position, tokenText.Length));
            }

            if (matchedDefinition.Regex.ToString() == TokenRegexDefinition.KWordBracketed)
                return new WordToken(match.Groups[1].Value, new TextSpan(Position + 1, match.Groups[1].Value.Length));
            return new WordToken(tokenText, new TextSpan(Position, tokenText.Length));
        }

        #endregion
    }
}