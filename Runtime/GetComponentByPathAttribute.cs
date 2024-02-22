﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SaintsField
{
    public class GetComponentByPathAttribute: PropertyAttribute, ISaintsAttribute
    {
        public SaintsAttributeType AttributeType => SaintsAttributeType.Other;
        public string GroupBy => "";

        // private static readonly Regex SlashContent = new Regex(@"(//?)([^/]+)");
        private static readonly Regex ContentSquareBracket = new Regex(@"([^[\]]*)\[([^\[\]]+)\]");

        public enum Locate
        {
            Child,      // "/"
            Descendant,       // "//"
            Root,  // "/"
            // Current,    // "."
            // other like /following-sibling::*, /preceding-sibling::*, ... is not supported
        }

        public struct Token
        {
            public Locate Locate;
            public string Node;
            // this is not index, but at this point it's OK.
            // e.g. [1], [last()], [*](has children), [position()>2], ...
            // at this point just number, last()
            // empty string for no index (do NOT use null)
            public string Index;

#if UNITY_EDITOR
            public override string ToString() => $"{Locate}::{EditorNodeToString(Node)}::{Index}";
            private static string EditorNodeToString(string node)
            {
                // ReSharper disable once ConvertSwitchStatementToSwitchExpression
                switch (node)
                {
                    case ".":
                        return "<CUR>";
                    case "..":
                        return "<PARENT>";
                    default:
                        return node;
                }
            }
#endif
        }

        // ReSharper disable InconsistentNaming
        public readonly Type CompType;
        public readonly IReadOnlyList<IReadOnlyList<Token>> Paths;
        public readonly bool ForceResign;
        public readonly bool ResignButton = true;
        // ReSharper enable InconsistentNaming

        public GetComponentByPathAttribute(string path, params string[] paths)
        {
            Paths = paths
                .Prepend(path)
                .Select(each =>
                {
                    IReadOnlyList<Token> result = ParsePath(each).ToArray();
#if SAINTSFIELD_DEBUG && SAINTSFIELD_DEBUG_DROW_PROCESS_GET_COMPONENT_BY_PATH
                    Debug.Log($"ParsePath: {each} => {string.Join(", ", result)}");
#endif
                    return result;
                })
                .ToArray();
        }

        public GetComponentByPathAttribute(EGetComp config, string path, params string[] paths): this(path, paths)
        {
            ForceResign = config.HasFlag(EGetComp.ForceResign);
            ResignButton = !config.HasFlag(EGetComp.NoResignButton);
        }

        public GetComponentByPathAttribute(Type compType, EGetComp config, string path, params string[] paths): this(config, path, paths)
        {
            CompType = compType;
        }

        public GetComponentByPathAttribute(EGetComp config, Type compType, string path, params string[] paths): this(config, path, paths)
        {
            CompType = compType;
        }

        public static IEnumerable<Token> ParsePath(string path)
        {
            // "./sth" equals "sth", relative, so, (child::sth)

            // `//` means "anywhere", to say, "a//b" means any b descendant of a (child::a, descendant::b)
            // ".//sth" means `sth` anywhere under current. (descendant::sth)

            // "..//sth" (:parent, descendant::sth)

            // ".." means parent. "a/../b (child::a, :parent, child::b)

            // "/sth" means from root (:root, child::sth), equals `/./sth`
            // "//sth" means `sth` directly under root (:root, child::sth); which is `/` + `/sth`
            // "///sth" means `sth` anywhere under root (:root, descendant::sth), equals `/.//sth`

            // string processPath = (path.StartsWith("./") || path.StartsWith("/"))
            //     ? path
            //     : $"./{path}";

            string processPath;
            if (path.StartsWith("/"))
            {
                yield return new Token
                {
                    Locate = Locate.Root,
                    Node = "/",
                    Index = "",
                };
                // ReSharper disable once ReplaceSubstringWithRangeIndexer
                string sub = path.Substring(1);
                processPath = sub.StartsWith("/")? sub: $"/{sub}";
            }
            else
            {
                // just because Regex
                processPath = $"/{path}";
            }

            Match[] matches = Regex.Matches(processPath, @"(//?)([^/]+)").ToArray();
            foreach (Match match in matches)
            {
                string slash = match.Groups[1].Value;
                string content = match.Groups[2].Value;
                string index = string.Empty;

                Match contentMatch = ContentSquareBracket.Match(content);
                if (contentMatch.Success)
                {
                    content = contentMatch.Groups[1].Value;
                    index = contentMatch.Groups[2].Value.Trim();
                }

                Locate locate;
                // ReSharper disable once ConvertSwitchStatementToSwitchExpression
                switch (slash)
                {
                    case "//":
                        locate = Locate.Descendant;
                        break;
                    case "/":
                        locate = Locate.Child;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(slash), slash, null);
                }

                yield return new Token
                {
                    Locate = locate,
                    Node = content,
                    Index = index,
                };
            }
        }
    }
}
