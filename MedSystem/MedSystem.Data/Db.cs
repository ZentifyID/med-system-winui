using Microsoft.Data.Sqlite;

namespace MedSystem.Data;

/// <summary>
/// Единая точка подключения к базе данных.
/// Схема полностью совместима с python-версией (med_system.db):
/// обе программы могут открывать один и тот же файл.
/// </summary>
public static class Db
{
    /// <summary>
    /// Путь к файлу базы. По умолчанию — рядом с исполняемым файлом.
    /// Упакованное (MSIX) приложение должно переопределить путь на папку
    /// LocalFolder при старте — см. App.xaml.cs.
    /// </summary>
    public static string DbPath { get; set; } =
        Path.Combine(AppContext.BaseDirectory, "med_system.db");

    /// <summary>Открывает соединение с включёнными внешними ключами.
    /// Foreign Keys=True в строке подключения надёжнее ручного PRAGMA:
    /// применяется и к соединениям из пула.</summary>
    public static SqliteConnection Open()
    {
        var conn = new SqliteConnection($"Data Source={DbPath};Foreign Keys=True");
        conn.Open();
        return conn;
    }
}
