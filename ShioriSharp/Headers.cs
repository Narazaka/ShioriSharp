﻿using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ShioriSharp {
    /** <summary>SHIORI Message Headers Container</summary> */
    [SourceGenerator.ReferenceHeadersShortcut(100)] // Reference* header (SHIORI/2.2-2.6,3.x)
    [SourceGenerator.HeaderShortcut("Charset", @"request/response header")]
    [SourceGenerator.HeaderShortcut("Sender", @"request/response header")]
    [SourceGenerator.HeaderShortcut("SecurityLevel", @"request header (SHIORI/2.2,2.6,3.x)")]
    [SourceGenerator.HeaderShortcut("ID", @"request header (SHIORI/2.5,3.x)")]
    [SourceGenerator.HeaderShortcut("Event", @"request header (SHIORI/2.2)")]
    [SourceGenerator.HeaderShortcut("Type", @"request header (GET Word SHIORI/2.0)")]
    [SourceGenerator.HeaderShortcut("Status", @"request header (SHIORI/3.0 SSP extension) / response header (GET Status SHIORI/2.0)")]
    [SourceGenerator.HeaderShortcut("Ghost", @"request header (NOTIFY OwnerGhostName SHIORI/2.0,2.3)")]
    [SourceGenerator.HeaderShortcut("Sentence", @"request header (SHIORI/2.0,2.3b) / response header (SHIORI/2.0,2.2,2.3b,2.4)")]
    [SourceGenerator.HeaderShortcut("To", @"request header (SHIORI/2.3b)")]
    [SourceGenerator.HeaderShortcut("Age", @"request header (SHIORI/2.3b)")]
    [SourceGenerator.HeaderShortcut("Surface", @"request/response header (SHIORI/2.3b)")]
    [SourceGenerator.HeaderShortcut("Word", @"request header (TEACH SHIORI/2.4) / response header (GET Word SHIORI/2.0)")]
    [SourceGenerator.HeaderShortcut("String", @"response header (GET String SHIORI/2.5)")]
    [SourceGenerator.HeaderShortcut("Value", @"response header (GET SHIORI/3.0)")]
    [SourceGenerator.HeaderShortcut("BalloonOffset", @"response header (SHIORI/2.0)")]
    public partial class Headers : Dictionary<string, string> {
        const string HeaderSeparator = ": ";
        static Regex ReferenceRe = new Regex(@"^Reference(\d+)$");

        /** <summary>request header (NOTIFY OtherGhostName SHIORI/2.3)</summary> */
#if NET5_0
        public IEnumerable<string>? GhostEx { get; init; }
#else
        public IEnumerable<string>? GhostEx { get; set; }
#endif

        public override string ToString() {
            var str = new StringBuilder();
            foreach (var pair in this)
                str.Append(pair.Key).Append(HeaderSeparator).Append(pair.Value).Append(Common.LineSeparator);
            return str.ToString();
        }

        public string? Get(string name) => TryGetValue(name, out var value) ? value : null;

        public void Set(string name, string? value) {
            if (value is null) {
                Remove(name);
            } else {
                this[name] = value;
            }
        }

        public string? Reference(int index) => Get($"Reference{index}");

        public void Reference(int index, string value) => Set($"Reference{index}", value);

        public string?[] References() {
            var indexes = new Dictionary<int, string>(Count);
            int maxIndex = -1;
            foreach (var pair in this) {
                var match = ReferenceRe.Match(pair.Key);
                if (match is not null && match.Success) {
                    var index = int.Parse(match.Captures[0].Value);
                    indexes[index] = pair.Key;
                    if (maxIndex < index)
                        maxIndex = index;
                }
            }
            var references = new string?[maxIndex + 1];
            foreach (var pair in indexes) {
                references[pair.Key] = this[pair.Value];
            }
            return references;
        }

        public KeyValuePair<string, string>? InvalidPair {
            get => this.FirstOrDefault(pair => pair.Key.Contains("\n") || pair.Value.Contains("\n"));
        }

        public bool Valid {
            get => InvalidPair is null;
        }

        public void Validate() {
            var invalidPair = InvalidPair;
            if (invalidPair is not null) {
                throw new InvalidOperationException($"header has \\n value [{((KeyValuePair<string, string>)invalidPair).Key}: {invalidPair.Value}]");
            }
        }
    }
}

