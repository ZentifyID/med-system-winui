using Microsoft.Data.Sqlite;
using MedSystem.Core.Models;

namespace MedSystem.Data.Repositories;

public static class GroupRepository
{
    /// <summary>Дата перевода групп на следующий курс (месяц, день).</summary>
    public static (int Month, int Day) AcademicYearRollover { get; set; } = (8, 15);

    public static List<Group> GetAll()
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id, name FROM groups ORDER BY name";
        using var reader = cmd.ExecuteReader();
        var result = new List<Group>();
        while (reader.Read())
            result.Add(new Group { Id = reader.GetInt64(0), Name = reader.GetString(1) });
        return result;
    }

    public static Group? GetById(long id)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id, name FROM groups WHERE id = $id";
        cmd.Parameters.AddWithValue("$id", id);
        using var reader = cmd.ExecuteReader();
        if (!reader.Read())
            return null;
        return new Group { Id = reader.GetInt64(0), Name = reader.GetString(1) };
    }

    public static long GetStudentCount(long groupId)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM students WHERE group_id = $id";
        cmd.Parameters.AddWithValue("$id", groupId);
        return Convert.ToInt64(cmd.ExecuteScalar());
    }

    public static void Insert(string name)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO groups (name) VALUES ($name)";
        cmd.Parameters.AddWithValue("$name", name);
        cmd.ExecuteNonQuery();
    }

    public static void Update(long id, string name)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE groups SET name = $name WHERE id = $id";
        cmd.Parameters.AddWithValue("$name", name);
        cmd.Parameters.AddWithValue("$id", id);
        cmd.ExecuteNonQuery();
    }

    /// <summary>Удаляет группу; при cascade=true сначала студентов — одной транзакцией.</summary>
    public static void Delete(long id, bool cascade = false)
    {
        using var conn = Db.Open();
        using var tx = conn.BeginTransaction();
        if (cascade)
        {
            using var delStudents = conn.CreateCommand();
            delStudents.CommandText = "DELETE FROM students WHERE group_id = $id";
            delStudents.Parameters.AddWithValue("$id", id);
            delStudents.ExecuteNonQuery();
        }
        using var delGroup = conn.CreateCommand();
        delGroup.CommandText = "DELETE FROM groups WHERE id = $id";
        delGroup.Parameters.AddWithValue("$id", id);
        delGroup.ExecuteNonQuery();
        tx.Commit();
    }

    /// <summary>Увеличивает первую цифру в названиях групп (11-А → 21-А).</summary>
    public static int IncrementFirstDigitInAllGroups()
    {
        using var conn = Db.Open();
        using var tx = conn.BeginTransaction();
        var count = IncrementGroups(conn);
        tx.Commit();
        return count;
    }

    /// <summary>
    /// После даты начала учебного года — один раз в год автоматически
    /// переводит группы на следующий курс. Возвращает число обновлённых групп.
    /// </summary>
    public static int CheckAndAutoIncrementGroups()
    {
        var now = DateTime.Now;
        var (month, day) = AcademicYearRollover;
        if (now.Month < month || (now.Month == month && now.Day < day))
            return 0;

        var currentYear = now.Year.ToString();

        using var conn = Db.Open();
        using var tx = conn.BeginTransaction();

        using var check = conn.CreateCommand();
        check.CommandText = "SELECT value FROM system_info WHERE key = 'last_group_increment_year'";
        var lastYear = check.ExecuteScalar() as string;
        if (lastYear == currentYear)
            return 0;

        var count = IncrementGroups(conn);

        using var save = conn.CreateCommand();
        save.CommandText = "INSERT OR REPLACE INTO system_info (key, value) VALUES ('last_group_increment_year', $year)";
        save.Parameters.AddWithValue("$year", currentYear);
        save.ExecuteNonQuery();

        tx.Commit();
        return count;
    }

    private static int IncrementGroups(SqliteConnection conn)
    {
        var groups = new List<(long Id, string Name)>();
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "SELECT id, name FROM groups";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                groups.Add((reader.GetInt64(0), reader.GetString(1)));
        }

        var toUpdate = new List<(long Id, string OldName, string NewName, int FirstDigit)>();
        foreach (var (id, name) in groups)
        {
            if (name.Length > 0 && char.IsDigit(name[0]))
            {
                var firstDigit = name[0] - '0';
                var newName = (firstDigit + 1).ToString() + name[1..];
                if (newName != name)
                    toUpdate.Add((id, name, newName, firstDigit));
            }
        }

        // По убыванию первой цифры, чтобы не нарушить UNIQUE
        toUpdate.Sort((a, b) => b.FirstDigit.CompareTo(a.FirstDigit));

        var count = 0;
        foreach (var (id, oldName, newName, _) in toUpdate)
        {
            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE groups SET name = $name WHERE id = $id";
                cmd.Parameters.AddWithValue("$name", newName);
                cmd.Parameters.AddWithValue("$id", id);
                cmd.ExecuteNonQuery();
                count++;
            }
            catch (SqliteException)
            {
                throw new InvalidOperationException(
                    $"Не удалось перевести группу '{oldName}': группа '{newName}' уже существует.");
            }
        }
        return count;
    }
}
