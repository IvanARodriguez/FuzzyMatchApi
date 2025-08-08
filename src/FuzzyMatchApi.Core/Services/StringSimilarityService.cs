
using FuzzyMatchApi.Core.Interfaces;
using System.Text.RegularExpressions;

namespace FuzzyMatchApi.Core.Services;

public class StringSimilarityService : IStringSimilarityService
{
    // Levenshtein distance and similarity algorithms
    public double CalculateJaroWinklerSimilarity(string source, string target)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            return 0;

        source = source.ToLowerInvariant().Trim();
        target = target.ToLowerInvariant().Trim();

        if (source == target)
            return 1.0;

        return JaroWinkler(source, target);
    }

    public double CalculateLevenshteinSimilarity(string source, string target)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            return 0;

        source = source.ToLowerInvariant().Trim();
        target = target.ToLowerInvariant().Trim();

        if (source == target)
            return 1.0;

        var distance = LevenshteinDistance(source, target);
        var maxLength = Math.Max(source.Length, target.Length);

        return maxLength == 0 ? 1.0 : 1.0 - ((double)distance / maxLength);
    }

    public double CalculateTokenSetRatio(string source, string target)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            return 0;

        var sourceTokens = TokenizeAndSort(source);
        var targetTokens = TokenizeAndSort(target);

        return CalculateLevenshteinSimilarity(sourceTokens, targetTokens);
    }

    public double CalculateWeightedSimilarity(string source, string target)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            return 0;

        var levenshtein = CalculateLevenshteinSimilarity(source, target);
        var jaroWinkler = CalculateJaroWinklerSimilarity(source, target);
        var tokenSet = CalculateTokenSetRatio(source, target);

        // Bonus for exact matches
        if (string.Equals(source.Trim(), target.Trim(), StringComparison.OrdinalIgnoreCase))
            return 1.0;

        // Bonus for substring matches
        var substringBonus = CalculateSubstringBonus(source, target);

        // Weighted combination
        var weightedScore = (levenshtein * 0.3) + (jaroWinkler * 0.4) + (tokenSet * 0.3) + substringBonus;

        return Math.Min(1.0, weightedScore);
    }

    private int LevenshteinDistance(string source, string target)
    {
        if (source.Length == 0) return target.Length;
        if (target.Length == 0) return source.Length;

        var matrix = new int[source.Length + 1, target.Length + 1];

        for (int i = 0; i <= source.Length; i++)
            matrix[i, 0] = i;

        for (int j = 0; j <= target.Length; j++)
            matrix[0, j] = j;

        for (int i = 1; i <= source.Length; i++)
        {
            for (int j = 1; j <= target.Length; j++)
            {
                int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;
                matrix[i, j] = Math.Min(
                    Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost);
            }
        }

        return matrix[source.Length, target.Length];
    }

    private double JaroWinkler(string s1, string s2)
    {
        if (s1 == s2) return 1.0;

        int len1 = s1.Length;
        int len2 = s2.Length;

        if (len1 == 0 || len2 == 0) return 0.0;

        int matchDistance = Math.Max(len1, len2) / 2 - 1;
        if (matchDistance < 0) matchDistance = 0;

        bool[] s1Matches = new bool[len1];
        bool[] s2Matches = new bool[len2];

        int matches = 0;
        int transpositions = 0;

        // Identify matches
        for (int i = 0; i < len1; i++)
        {
            int start = Math.Max(0, i - matchDistance);
            int end = Math.Min(i + matchDistance + 1, len2);

            for (int j = start; j < end; j++)
            {
                if (s2Matches[j] || s1[i] != s2[j]) continue;
                s1Matches[i] = s2Matches[j] = true;
                matches++;
                break;
            }
        }

        if (matches == 0) return 0.0;

        // Count transpositions
        int k = 0;
        for (int i = 0; i < len1; i++)
        {
            if (!s1Matches[i]) continue;
            while (!s2Matches[k]) k++;
            if (s1[i] != s2[k]) transpositions++;
            k++;
        }

        double jaro = ((double)matches / len1 + (double)matches / len2 +
                      (double)(matches - transpositions / 2) / matches) / 3.0;

        // Jaro-Winkler
        if (jaro < 0.7) return jaro;

        int prefix = 0;
        for (int i = 0; i < Math.Min(len1, len2) && i < 4; i++)
        {
            if (s1[i] == s2[i]) prefix++;
            else break;
        }

        return jaro + (0.1 * prefix * (1.0 - jaro));
    }

    private string TokenizeAndSort(string input)
    {
        var tokens = Regex.Split(input.ToLowerInvariant(), @"\W+")
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .OrderBy(t => t)
            .ToArray();

        return string.Join(" ", tokens);
    }

    private double CalculateSubstringBonus(string source, string target)
    {
        source = source.ToLowerInvariant().Trim();
        target = target.ToLowerInvariant().Trim();

        if (source.Contains(target) || target.Contains(source))
            return 0.1;

        // Check for significant word overlaps
        var sourceWords = source.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var targetWords = target.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var commonWords = sourceWords.Intersect(targetWords).Count();
        var totalWords = Math.Max(sourceWords.Length, targetWords.Length);

        if (totalWords > 0 && commonWords > 0)
            return Math.Min(0.15, (double)commonWords / totalWords * 0.15);

        return 0;
    }
}
