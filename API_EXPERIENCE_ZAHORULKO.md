# Мій досвід користування framework / API (Загорулько)

> ⚠️ ЧЕРНЕТКА. Розділи з позначкою **[ЗАПОВНИТИ]** мають бути написані мною особисто:
> що я реально відчувала, що реально сказала Каті, які prompt-и реально давала AI.
> Після заповнення цей блок видалити.

Роль: AI-assisted Framework Developer. Архітектор framework — Єфімова.
Цей документ — моє особисте пояснення; `AI_USAGE.md` архітектора його не замінює.

## 1. Налаштування: як я підключала свої стани до framework

### Нова поведінка ворога через extension API (`IEnemyBehaviour`)
1. Створити компонент, що реалізує `IEnemyBehaviour` (`Initialize`, `Enter`, `Execute`, `Exit`, `Restart`, `IsBusy`).
   У мене це `ChargeBehaviour`: windup (миготіння червоним) → ривок до гравця → повернення → cooldown → знову готовий.
2. Повісити його на ворога разом з `ChargerEnemy` (наслідує `BaseEnemy`). `ChargerEnemy` бере поведінку через
   `GetComponent<IEnemyBehaviour>()`, тобто не знає, що це саме Charge — поведінку можна замінити іншою.
3. Перевірка в рівні: підійти до `ChargerEnemy` у `VerticalSlice_Level` / `level2` і дивитися Console:
   `[EnemyBehaviour] ENTER` → `EXECUTE` → `EXIT` → `RESTART`, після чого ворог знову може атакувати.

### Interaction-сценарій гравця (Animation Event)
- Клавіша **E** → `PlayerInteraction` → `PlayerAnimationRelay.TriggerAction()` → стан `Player_Action`.
- На кліпі `Player_Action.anim` дві події:
  `OnInteractionImpact` (0.25 с, момент дії, **один** `Debug.Log`) і `OnInteractionFinished` (кінець кліпу).
- Перехід `Player_Action → Idle` тепер по Exit Time = 1 (кліп програється повністю), повторний тригер під час дії ігнорується.

### Hurt / Death ворога
- `EnemyReactions` на префабі `Enemy_ProximityMine`. Тестові клавіші: **J** — Hurt, **L** — Death
  (спрацьовують лише для ворогів у радіусі 8 від гравця).
- Death: логіка й колайдер ворога вимикаються (`BaseEnemy.SetLogicActive(false)`), грає `Anim_Mine_Death`,
  подія `OnDeathAnimationFinished` ховає спрайт, через 1.5 с ворог відроджується (можна тестувати повторно).
- Hurt: кліп більше не зациклений, подія `OnHurtAnimationFinished`, повернення в Idle після кінця кліпу.

## 2. Незручності framework, які я помітила

| Що | Чому це заважало | Що зроблено в моєму PR |
|---|---|---|
| Файл `ChargeEnemy.cs` містив клас `ChargerEnemy` | Unity вимагає однакову назву файлу й класу — скрипт на префабі не підключався | Перейменовано на `ChargerEnemy.cs` (GUID збережено) |
| `IEnemyBehaviour` не мав `Initialize` / `IsBusy` | Ворог мусив знати конкретний клас `ChargeBehaviour`, тобто «extension point» не працював як розширення | Додано в інтерфейс |
| Animation Event отримують **усі** компоненти на об'єкті з Animator | `OnInteractionEvent` був і в `PlayerAnimationRelay`, і в `AnimationFeedbackHandler` → ризик дубльованих логів | Окремі імена подій, обробник один (`PlayerInteraction`) |
| Тригер `Action` гравця ніхто не викликав | `AnimationFeedbackHandler` з клавішею E висів на міні, в Animator міни параметра `Action` немає | Тестовий тригер перенесено на гравця, `AnimationFeedbackHandler` видалено |
| Death ворога = scale → 0 без виходу | Невидима міна продовжувала «вбивати» гравця, повторно протестувати не можна | `EnemyReactions` + `SetLogicActive` + revive |
| `PlayerHealth` робив усе (HP, knockback, падіння, respawn) | Важко зрозуміти, де що змінювати | Розділено на `PlayerHealth` / `PlayerHitHandler` / `PlayerRespawn` |
| Різні версії Unity у команді (6000.3.24f1 / 6000.6.3f1) | Unity перезаписує налаштування → конфлікти в `ProjectSettings`/URP | Треба домовитись про одну версію |

**[ЗАПОВНИТИ]** — що з цього я відчула найбільше / що ще було незручно особисто мені під час роботи.

## 3. Feedback, повідомлений архітектору

**[ЗАПОВНИТИ]** — що саме і коли я написала Каті (повідомлення / коментар у PR). Пропоновані пункти, якщо ще не надіслано:
1. Назва файлу має збігатися з класом (`ChargeEnemy.cs` → `ChargerEnemy.cs`).
2. `IEnemyBehaviour` потребує `Initialize` і `IsBusy`, інакше ворог залежить від конкретної поведінки.
3. Animation Events: домовитись, що кожну подію обробляє рівно один компонент.
4. `PlayerAnimationRelay.OnInteractionEvent` більше не використовується — можна прибрати.
5. Одна версія Unity для всієї команди.

## 4. Мої AI-ітерації

**[ЗАПОВНИТИ / ВІДРЕДАГУВАТИ]** — 1–2 власні ітерації у форматі «prompt → що дав AI → що було не так → що я змінила → як перевірила». Приклад із цієї роботи:

**Ітерація 1 — відгук не збігався з назвами в проєкті.**
- Prompt: дала AI список правок від викладача і попросила їх внести.
- Результат: AI не знайшов `PlayerFoxy`, `Victory.anim`, `AI_USAGE.md` і спочатку зупинився з питанням.
- Що змінила: уточнила, що це наш проєкт, і ми зіставили пункти з реальними файлами (`Player_Action.anim`, міна з flash/scale→0, `PlayerHealth` з гілки Каті).
- Перевірка: кожен пункт відгуку прив'язаний до конкретного файлу в PR.

**Ітерація 2 — перевірка не лише компіляцією.**
- Prompt: перевірити, що зміни реально працюють у Unity.
- Результат: AI запустив Unity в batchmode (сцени без Missing Script) і тимчасові PlayMode-тести.
  Перший запуск тестів не скомпілювався (помилка в asmdef), після виправлення — тести підтвердили
  Hurt → Death → revive, один лог interaction і respawn гравця. Тимчасові тести з PR прибрані.
- Що я перевірила сама в Play Mode: **[ЗАПОВНИТИ]**.
