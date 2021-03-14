// <copyright file="Leaderboards.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce
{
    using System.Collections.Generic;
    using Internal;

    /// <summary>
    /// Provides access to leaderboards registered via the CloudOnce Editor.
    /// This file was automatically generated by CloudOnce. Do not edit.
    /// </summary>
    public static class Leaderboards
    {
        private static readonly UnifiedLeaderboard s_arena = new UnifiedLeaderboard("Arena",
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_TVOS)
            ""
#elif !UNITY_EDITOR && UNITY_ANDROID && CLOUDONCE_GOOGLE
            "CgkIm7Xjl_oXEAIQAA"
#else
            "Arena"
#endif
            );

        public static UnifiedLeaderboard Arena
        {
            get { return s_arena; }
        }

        private static readonly UnifiedLeaderboard s_diamond = new UnifiedLeaderboard("Diamond",
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_TVOS)
            ""
#elif !UNITY_EDITOR && UNITY_ANDROID && CLOUDONCE_GOOGLE
            "CgkIm7Xjl_oXEAIQAQ"
#else
            "Diamond"
#endif
            );

        public static UnifiedLeaderboard Diamond
        {
            get { return s_diamond; }
        }

        private static readonly UnifiedLeaderboard s_orbit = new UnifiedLeaderboard("Orbit",
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_TVOS)
            ""
#elif !UNITY_EDITOR && UNITY_ANDROID && CLOUDONCE_GOOGLE
            "CgkIm7Xjl_oXEAIQAg"
#else
            "Orbit"
#endif
            );

        public static UnifiedLeaderboard Orbit
        {
            get { return s_orbit; }
        }

        private static readonly UnifiedLeaderboard s_pinball = new UnifiedLeaderboard("Pinball",
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_TVOS)
            ""
#elif !UNITY_EDITOR && UNITY_ANDROID && CLOUDONCE_GOOGLE
            "CgkIm7Xjl_oXEAIQAw"
#else
            "Pinball"
#endif
            );

        public static UnifiedLeaderboard Pinball
        {
            get { return s_pinball; }
        }

        private static readonly UnifiedLeaderboard s_warp = new UnifiedLeaderboard("Warp",
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_TVOS)
            ""
#elif !UNITY_EDITOR && UNITY_ANDROID && CLOUDONCE_GOOGLE
            "CgkIm7Xjl_oXEAIQBA"
#else
            "Warp"
#endif
            );

        public static UnifiedLeaderboard Warp
        {
            get { return s_warp; }
        }

        private static readonly UnifiedLeaderboard s_clock = new UnifiedLeaderboard("Clock",
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_TVOS)
            ""
#elif !UNITY_EDITOR && UNITY_ANDROID && CLOUDONCE_GOOGLE
            "CgkIm7Xjl_oXEAIQBQ"
#else
            "Clock"
#endif
            );

        public static UnifiedLeaderboard Clock
        {
            get { return s_clock; }
        }

        public static string GetPlatformID(string internalId)
        {
            return s_leaderboardDictionary.ContainsKey(internalId)
                ? s_leaderboardDictionary[internalId].ID
                : string.Empty;
        }

        private static readonly Dictionary<string, UnifiedLeaderboard> s_leaderboardDictionary = new Dictionary<string, UnifiedLeaderboard>
        {
            { "Arena", s_arena },
            { "Diamond", s_diamond },
            { "Orbit", s_orbit },
            { "Pinball", s_pinball },
            { "Warp", s_warp },
            { "Clock", s_clock },
        };
    }
}