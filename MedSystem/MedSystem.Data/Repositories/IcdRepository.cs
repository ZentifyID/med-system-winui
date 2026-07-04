using MedSystem.Core.Models;

namespace MedSystem.Data.Repositories;

public static class IcdRepository
{
    /// <summary>
    /// Поиск по коду или названию (без учёта регистра, включая кириллицу).
    /// Фильтрация выполняется в C#, т.к. встроенный LOWER SQLite
    /// не работает с кириллицей.
    /// </summary>
    public static List<IcdCode> Search(string query, int limit = 30)
    {
        var all = GetAll();
        if (string.IsNullOrWhiteSpace(query))
            return all.Take(limit).ToList();

        var q = query.Trim().ToLowerInvariant();
        return all
            .Where(c => c.Code.ToLowerInvariant().Contains(q)
                     || c.Name.ToLowerInvariant().Contains(q))
            .Take(limit)
            .ToList();
    }

    public static List<IcdCode> GetAll()
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT code, name FROM icd_codes ORDER BY code";
        using var reader = cmd.ExecuteReader();
        var result = new List<IcdCode>();
        while (reader.Read())
            result.Add(new IcdCode { Code = reader.GetString(0), Name = reader.GetString(1) });
        return result;
    }

    /// <summary>Возвращает false, если код уже существует.</summary>
    public static bool Insert(string code, string name)
    {
        try
        {
            using var conn = Db.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO icd_codes (code, name) VALUES ($code, $name)";
            cmd.Parameters.AddWithValue("$code", code);
            cmd.Parameters.AddWithValue("$name", name);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Microsoft.Data.Sqlite.SqliteException)
        {
            return false;
        }
    }

    /// <summary>Возвращает false, если новый код конфликтует с существующим.</summary>
    public static bool Update(string oldCode, string newCode, string newName)
    {
        try
        {
            using var conn = Db.Open();
            if (oldCode != newCode)
            {
                using var check = conn.CreateCommand();
                check.CommandText = "SELECT 1 FROM icd_codes WHERE code = $code";
                check.Parameters.AddWithValue("$code", newCode);
                if (check.ExecuteScalar() != null)
                    return false;
            }
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE icd_codes SET code = $newCode, name = $name WHERE code = $oldCode";
            cmd.Parameters.AddWithValue("$newCode", newCode);
            cmd.Parameters.AddWithValue("$name", newName);
            cmd.Parameters.AddWithValue("$oldCode", oldCode);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Microsoft.Data.Sqlite.SqliteException)
        {
            return false;
        }
    }

    public static void Delete(string code)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM icd_codes WHERE code = $code";
        cmd.Parameters.AddWithValue("$code", code);
        cmd.ExecuteNonQuery();
    }
}
