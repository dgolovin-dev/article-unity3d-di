# Связывание объектов в Unity3d (Service Locator, Dependency Injection)

<img src ="https://unity3d.com/profiles/unity3d/themes/unity/images/pages/branding_trademarks/unity-mwu-black.png" width="200"/>

Привет. Сегодня поговорим о *связывании объектов* в Unity.
Я покажу несколько способов разбить ваше приложение на на части и потом связать эти части между собой. Цель этих манипуляций благая - упростить поддержку куда и его модифицируемость.

Я покажу как для этих целей использовать паттерны:
- Service Locator
- Dependency Injection

В качестве примера, я буду использовать простенькую [asteroids-like игру](https://ru.wikipedia.org/wiki/Asteroids).
Полный код примера доступеyн [>здесь<](https://github.com/dgolovin-dev/article-unity3d-di).

## Прямое связывание

Когда вы начинаете разрабатывать проект в Unity, вы сразу начинаете использовать *прямое cвязывание* в Unity.
Вы просто проставляете ссылки на объекты в полях инспектора Unity и в рантайме движок Unity, корректно проставит ссылки на созданные объекты до вызова метода "Awake()". 

<img src='https://github.com/dgolovin-dev/article-unity3d-di/raw/main/docs/direct.png' width='400'/>

*Полный исходный код доступен в папке `Assets/direct` в [репозитории](https://github.com/dgolovin-dev/article-unity3d-di)*

Однако с таким подходом есть проблемы:
1. Вы должны создать и сохранить все GameObject и Component в одной и той же сцене. И это будет огромная сцена. Очень легко зацепить и сломать что-то в сцене. Также параллельная работа над такой сценой несколькими разработчиками практически невозможна из-за конфликтов в VCS(git, svn, etc).
2. Тяжело отслеживать ссылки, повышается вероятность затереть или потерять какую-нибудь ссылку. Особенно при рефакторинге (например, замена класса компонента на другой).
3. Становится тяжелее распределять отвественность между компонентами. Подсознательно, вы будете стараться уменьшить число взаимосвязей между компонентами и создавать большие компоненты со смешанными отвественностями (god-objects).
4. Этот подход ведет к созданию дубликатов конфигураций объектов, когда вам нужно несколько одинаковых объектов. Это еще больше усложняет поддержку. 

В конце концов, ваш проект станет похож на клубок спутанных проводов. Тяжело будет внести правки и ничего не сломать.

<img src='https://www.staticelectrics.com.au/wp-content/uploads/2021/02/faulty-wiring.jpg' width='400'/>

Первым шагом к решению этой проблемы будет использование *Prefab*. Вы создаете *prefab* для каждого объекта на сцене, имеющего много внутренних ссылок на свои части. Вы переносите часть конфигурации объекта в отдельный файл и сцена становится меньше. Уменьшается дублирование конфигурации. У вас появляется возможность параллельной разработки, ведь теперь можно модифицировать префаб не трогая сцену. Однако связи между объектами, которые создаются из префабов, все еще сохранены в сцене. И все еще существует проблема пареллельной модификации сцены, сцена все еще будет ломаться часто. Желательно убрать из сцены и эти связи.

## Service Locator

Совет: создавайте маленькие/средние по размеру префабы и храните минимум связей между ними в сцене.

Также, вы можете создавать GameObject из префабов в рантайме без сохранения их на сцене. Это удобно для UI окон и объектов порождаемых в рантайме(например всякие снаряды или мобы). Вы можете использовать стандартный подход с Builder и связать объекты в момент их создания. Однако в Unity есть более удобный подход, который позволит вам обойтись без написания своих Builder. Это Service Locator. Он позволяет инвертировать зависимости и с его помощью компоненты сами могут найти необходимые зависимости. Это уменьшает связанность и упрощает поддержку кода.

Service Locator В Unity представлен методами `GameObject.Find*` и `Component.GetComponent*`. 
Unity помещает каждый созданный объект во внутренний реестр и позволяет вести поиск по нему с использованием этих методов. 

*Полный код пример находится в папке `Assets/locator` в [репозитории](https://github.com/dgolovin-dev/article-unity3d-di)*

Давайте рассмотрим несколько кусочков:

1. Создание объектов из префобов в `ObjectFactory.Awake`:

```C#
  public class ObjectFactory: MonoBehaviour {
    [SerializeField]
    private GameObject[] prefabs;
    
    private void Awake() {
      foreach (var p in prefabs) {
        Instantiate(p, transform, false);
      }
    }
    
  }
}
```

2. Поиск и связывание в `AsteroidManager`:

```C#
  public class AsteroidManager : MonoBehaviour {
    [SerializeField][NotEditable]
    private List<Asteroid> asteroids;
    ...
    void Start() {
      asteroids = GameObject.FindGameObjectsWithTag("asteroid")
        .Select(i => i.GetComponent<Asteroid>())
        .OrderBy(i => i.level)
        .ThenBy(i => i.name)
        .ToList();
      foreach (var a in asteroids) {
        a.onDeath.AddListener(OnAsteroidDead);
      }
    }
    ...
  }
```

3. Поиск и связывание `GameManager`.

```C#
  public class GameManager: MonoBehaviour { 
    [SerializeField][NotEditable]
    private Starship starship;
    [SerializeField][NotEditable]
    private AsteroidManager asteroidManager;

    private void Start() {
      starship = GameObject.FindGameObjectWithTag("starship").GetComponent<Starship>();
      asteroidManager = GameObject.FindGameObjectWithTag("asteroidManager").GetComponent<AsteroidManager>();
      starship.onDeath.AddListener(LoseGame);
      asteroidManager.onAllDead.AddListener(WinGame);
      StartGame();
    }
```

Несколько важных замечаний:

- Создание новых объектов происходит в ObjectFactory. Сцена практически пустая. Это хорошо для командной работы. 
(помогает избежать конфликтов).

- Я использую `GameObject.FindGameObjectWithTag(...).GetComponent<...>()` 
вместо `GameObject.FindObjectOfType<...>()` Методы поиска по тегу работают гораздо быстрее. Также рекомендую избегать использования  `GetComponentsInChildren`.
Also, I recommend to avoid using `GetComponentsInChildren`. `FindObjectOfType` and `GetComponentsInChildren` 
go through the full hierarchy and call `GetComponent` on every game object
  (This is very slow.).

- It makes sense to call the methods-locators(`Find*`,`GetComponent*`)
as rare as possible. Ideally, only once in the `Awake` or `Start`. 
If you place such calls in the `Update`, it will kill the performance of your game.

- Pay attention to the order of the game objects creation
and their initialization (1 -> 2 -> 3).
Because of this initialization order, locating and binding is
in the `Start` methods (the next frame after the instantiation).
And that is why `ObjectFactory` is the last in 
the creation list. It must be initialized last (it needs the other objects for that).


These points make the code a bit more complex, 
and make you think about the initialization order.
But it allows you to split your scene into independent
prefabs and work comfortably on the project in a parallel manner.
You artifacts will be more stable with this approach 
than with the direct binding.

## Dependency Injection

When your project is becoming bigger,
it makes harder to track the initialization order.

For example, you call a method-locator, 
but the target object is not instantiated yet.

Of course, you can write a coroutine like that:

```C#
private IEnumerator Start() {
  while(starship == null) {
    starship = GameObject.FindGameObjectWithTag("starship")?.GetComponent<Starship>();
    if(starship == null) {
      yield return null; 
    }
  }
}
```

It will work, but your code will 
become dirty after some time and it will be
hard to modify and fix bugs.

The good solution, in this case, is to use the `Dependency Injection`. 
When you write a class you just declare, 
that it needs some other objects and, in runtime,
the external system will provide these objects 
and notify about it. 
It is hard to understand abstractly, so let's see the next code snippet:

```c#
  public class GameManager: SceneContextMonoBehaviour {
    ...
    [Inject][SerializeField][NotEditable]
    private Starship starship;
    [Inject][SerializeField][NotEditable]
    private AsteroidManager asteroidManager;
    ...

    [AfterInject]
    private void AfterInject() {
      starship.onDeath.AddListener(LoseGame);
      asteroidManager.onAllDead.AddListener(WinGame);
      StartGame();
    }
    ...
  }
```

*The full example you can find here `Assets/context` [>git repo<](https://github.com/dgolovin-dev/article-unity3d-di).*

I declare dependencies using the attribute `[Inject]`
(`[SerializeField][NotEditable]` are optional, they helps to track dependencies in the Inspector).
The *context* reads these attributes in the runtime and binds objects.

When the *context* resolves all the dependencies, 
it calls the callback with the attribute `[AfterInject]`.

Pay attention to the superclass `SceneContextMonoBehaviour`.
It 'says' to the context to inject the dependencies
and add this object to the context 
for injecting dependencies of other objects.

```C# 
  void Awake() {
    var context = SceneContextHolder.GetContext(gameObject.scene);
    context.Inject(this); // resolves the dependencies
    context.Add(this); // adds to context
  }
```

Let's talk about **context**.
Context is the dependency injection system.
Some people prefer to name it 'container'.
It consists of:

- The registry. When you call `context.Add`, 
you add the component to the registry and it becomes
available for the dependency injection.
- The locator. It allows to find objects in registry
by some features (in this example - by class).
- The dependency injector. `context.Inject`
finds all fields with the attribute `[Inject]`,
and inject the necessary dependencies 
in these fields. It will wait if the target 
dependency is not available yet.
When it finishes, it will call a callback 
with the attribute `[AfterInject]`.

The *context* may live all alone,
but, often, it is better to bind it to other parts 
of your system. 
In this example, the context is bound to the scene.
I highly recommend to see the implementation of `Context`
and use this approach in your projects.

This implementation is very small and rough
with the purpose to show you the main concepts of DI.
You can look at on the VContainer and ZInject as the
other mature implementations of this approach.

# [[Author]](/)

<script src='/assets/comments.js'></script>
