<p align="center">
    <img width="96" src="source/ToadTools.Desktop/Resources/logo.png" alt="ToadTools" />
    <h1 align="center">ToadTools</h1>
    <p align="center">Программа в трее, которая ставит и обновляет плагин <a href="https://github.com/Alexpivo394/ToadTools">ToadTools</a> для Autodesk Revit</p>
</p>

## Установка

Скачайте `ToadTools-App-*.msi` из [последнего релиза](https://github.com/Alexpivo394/ToadTools.Desktop/releases/latest) и запустите. Права администратора не нужны.

Программа сидит в трее, скачивает плагин для всех найденных версий Revit и ставит новые версии, как только закрыт последний Revit. Окно настроек — щелчок по значку: частота проверки, версии Revit, автозапуск, уведомления, прокси. Подробно — на странице вики [«Установка и обновление»](https://github.com/Alexpivo394/ToadTools/wiki/Install).

Изменения по версиям — в [Changelog.md](Changelog.md).

## Как работает

1. Раз в N часов программа спрашивает GitHub о последних релизах двух репозиториев (с ETag — ответ «ничего не изменилось» не тратит лимит):
   - [ToadTools](https://github.com/Alexpivo394/ToadTools/releases/latest) — плагин: `ToadTools-X.Y.Z.zip` + `.zip.sig`;
   - этот репозиторий — сама программа: `ToadTools-App-X.Y.Z.msi` + `.msi.sig`.
2. Новый файл скачивается в `%LocalAppData%\ToadTools\downloads` (с докачкой) и проверяется подписью ECDSA P-256.
3. Архив плагина распаковывается в `%LocalAppData%\ToadTools\staging\X.Y.Z`.
4. Нет ни одного `Revit.exe` текущего пользователя — плагин ставится в `%AppData%\Autodesk\Revit\Addins\{год}`: новая папка кладётся рядом, затем `ToadTools` → `ToadTools.old`, `ToadTools.new` → `ToadTools`. Если файлы держит Revit, всё откатывается. Revit открыт — программа ждёт события завершения процесса от Windows, не опрашивая.
5. Новая версия программы ставится тихо (`msiexec /qn`): программа закрывается, MSI заменяет файлы и запускает её снова.

> [!IMPORTANT]
> Формат архива плагина — договорённость с репозиторием ToadTools: `{год}/ToadTools.addin` и `{год}/ToadTools/**`, как папка `publish` сборки плагина. Её проверяют тесты `PluginArchiveTests`; меняя архив в ToadTools, меняйте и программу.

## Разработка

Нужны .NET 10 SDK и .NET Framework 4.8 Developer Pack.

| Папка | Что там |
|---|---|
| `source/ToadTools.Desktop.Core` | Логика без интерфейса: релизы, скачивание, подпись, установка плагина, ожидание Revit, настройки |
| `source/ToadTools.Desktop` | WPF-программа (`ToadTools.exe`): трей, уведомления, окно настроек на [ToadDesign](https://github.com/Alexpivo394/ToadDesign) |
| `tests/ToadTools.Desktop.Tests` | Тесты ядра: `dotnet run --project tests/ToadTools.Desktop.Tests -c Release` |
| `install` | Установщик на WixSharp с окном на ToadDesign (MSI embedded UI) |
| `build` | Сборка: `cd build; dotnet run -c Release -- pack` → `output/ToadTools-App-X.Y.Z.msi` и `.sig` |

Debug-сборка принимает `--feed <url>` (релиз плагина) и `--app-feed <url>` (релиз программы) вместо GitHub и сама себя не обновляет.

### Подпись

Приватный ключ — секрет `SIGNING_KEY` в этом репозитории и в ToadTools (один ключ), локально `dotnet user-secrets set Publish:SigningKey <ключ>` в `build`. Публичный ключ зашит в `source/ToadTools.Desktop.Core/Security/PublicKey.cs`. Новая пара: `dotnet run --project build -- keygen <файл>`. Сменить ключ можно только так: сначала выпустить программу с новым публичным ключом, подписанную старым.

### Выпуск

1. Раздел `# X.Y.Z` в начале `Changelog.md`.
2. Тег `X.Y.Z` или **Actions → Publish release → Run workflow**.

Версия программы — тег этого репозитория, независимо от версии плагина. Установленные программы обновляются до последнего релиза.

## Лицензия

MIT
