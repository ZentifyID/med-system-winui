using MedSystem.Core.Models;

namespace MedSystem.Data.Repositories;

public static class StudentRepository
{
    public static List<Student> GetAll()
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT s.id, s.group_id, g.name, s.last_name, s.first_name, s.middle_name,
                   s.birth_date, s.oms, s.address,
                   s.sanminimum_date, s.medical_exam_date, s.fluorography_date
            FROM students s
            JOIN groups g ON s.group_id = g.id
            ORDER BY s.last_name, s.first_name, s.middle_name
            """;
        using var reader = cmd.ExecuteReader();
        var result = new List<Student>();
        while (reader.Read())
        {
            result.Add(new Student
            {
                Id = reader.GetInt64(0),
                GroupId = reader.GetInt64(1),
                GroupName = reader.GetString(2),
                LastName = reader.GetString(3),
                FirstName = reader.GetString(4),
                MiddleName = reader.GetString(5),
                BirthDate = reader.GetString(6),
                Oms = reader.GetString(7),
                Address = reader.GetString(8),
                SanminimumDate = reader.GetString(9),
                MedicalExamDate = reader.GetString(10),
                FluorographyDate = reader.GetString(11),
            });
        }
        return result;
    }

    public static Student? GetById(long id)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT s.id, s.group_id, g.name, s.last_name, s.first_name, s.middle_name,
                   s.birth_date, s.oms, s.address,
                   s.sanminimum_date, s.medical_exam_date, s.fluorography_date
            FROM students s
            JOIN groups g ON s.group_id = g.id
            WHERE s.id = $id
            """;
        cmd.Parameters.AddWithValue("$id", id);
        using var reader = cmd.ExecuteReader();
        if (!reader.Read())
            return null;
        return new Student
        {
            Id = reader.GetInt64(0),
            GroupId = reader.GetInt64(1),
            GroupName = reader.GetString(2),
            LastName = reader.GetString(3),
            FirstName = reader.GetString(4),
            MiddleName = reader.GetString(5),
            BirthDate = reader.GetString(6),
            Oms = reader.GetString(7),
            Address = reader.GetString(8),
            SanminimumDate = reader.GetString(9),
            MedicalExamDate = reader.GetString(10),
            FluorographyDate = reader.GetString(11),
        };
    }

    public static void Insert(Student s)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO students (
                group_id, last_name, first_name, middle_name, birth_date,
                oms, address, sanminimum_date, medical_exam_date, fluorography_date
            ) VALUES (
                $groupId, $lastName, $firstName, $middleName, $birthDate,
                $oms, $address, $sanminimumDate, $medicalExamDate, $fluorographyDate
            )
            """;
        AddParameters(cmd, s);
        cmd.ExecuteNonQuery();
    }

    public static void Update(Student s)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            UPDATE students SET
                group_id = $groupId, last_name = $lastName, first_name = $firstName,
                middle_name = $middleName, birth_date = $birthDate, oms = $oms,
                address = $address, sanminimum_date = $sanminimumDate,
                medical_exam_date = $medicalExamDate, fluorography_date = $fluorographyDate
            WHERE id = $id
            """;
        AddParameters(cmd, s);
        cmd.Parameters.AddWithValue("$id", s.Id);
        cmd.ExecuteNonQuery();
    }

    public static void Delete(long id)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM students WHERE id = $id";
        cmd.Parameters.AddWithValue("$id", id);
        cmd.ExecuteNonQuery();
    }

    private static void AddParameters(Microsoft.Data.Sqlite.SqliteCommand cmd, Student s)
    {
        cmd.Parameters.AddWithValue("$groupId", s.GroupId);
        cmd.Parameters.AddWithValue("$lastName", s.LastName);
        cmd.Parameters.AddWithValue("$firstName", s.FirstName);
        cmd.Parameters.AddWithValue("$middleName", s.MiddleName);
        cmd.Parameters.AddWithValue("$birthDate", s.BirthDate);
        cmd.Parameters.AddWithValue("$oms", s.Oms);
        cmd.Parameters.AddWithValue("$address", s.Address);
        cmd.Parameters.AddWithValue("$sanminimumDate", s.SanminimumDate);
        cmd.Parameters.AddWithValue("$medicalExamDate", s.MedicalExamDate);
        cmd.Parameters.AddWithValue("$fluorographyDate", s.FluorographyDate);
    }
}
