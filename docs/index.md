# Object Binding in Unity3d (Service locator, dependency injection)

Hello. Today we are talking about *object binding* in Unity.
I am going to show some efficient ways on how to split your app parts and efficiently deal with the dependencies among these parts.

I will show how to use these patterns:
- Service Locator
- Dependency Injection

As an example, I will use the code of an asteroid-like game.
So, I advice you to clone the git repo and check the full examples out. 


## Direct binding

When you start developing project in Unity, 
you start with *direct binding* of components.
You just set the references in the fields in the Inspector View 
for the components on a scene. And in the runtime Unity Engine 
will set the correct references to the new created components before the "Awake()" call.


![direct example](https://media.githubusercontent.com/media/dgolovin-dev/article-unity3d-di/main/docs/direct.png)
*Check out the full example of this approach in the directory `Assets/direct`.*

There are a lot of problems with this approach. For example:
1. You should create and store all the game objects and components in the same
scene. It will be a huge scene. It is easy to break this scene and it is impossible
to modify in a parallel manner when you are in a team. 
In the most cases it is impossible to solve conflicts when you use VCS (git, svn, etc).
2. It is hard to track and maintain so many references. 
3. Sometimes, it makes harder to distribute responsibility between components. 
Subconsciously, you will tend to reduce the count of references and create big god-objects.
4. It creates configuration "duplicates" on the scene when you need several 
identical objects.

Overall, you project start looking like a bunch of tangled wires.
It will be hard to modify something and don't break it. 

![wires](https://www.staticelectrics.com.au/wp-content/uploads/2021/02/faulty-wiring.jpg)

If you create a **prefab** for every internally highly connected game object, you will move many links from the scene to the prefabs. It will become easier to maintain and it will unlock a parallel work.
But this is not enough. If you keep references among prefabs in a scene, you still have a problem with concurrent modification.
You scene will be an unstable point and it will break very often.
It better to remove even these links from your scene.


## Service Locator

Best practice: create middle/small prefabs and keep minimum of references 
among prefabs in your scenes.

You can instantiate game objects from prefabs in the code.
Then you can link these new objects in the code too
or use a pattern Service Locator 
and let your components to resolve their external references. 
The second approach is better because it decrease coupling 
and increase maintainability.

The standard Service Locator in Unity is available through methods 
`GameObject.Find*` и `Component.GetComponent*`. 
Unity places every instantiated component and game object in 
its internal registry and allow you to find(locate) them using these methods. 


*Check out the full example of this approach in the directory `Assets/locator`.*

Let's see some code snippets:

1. Game objects instantiation in `ObjectFactory.Awake`:

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

2. Search and binding in `AsteroidManager`:

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

3. Search and binding in GameManager.

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

There are some important points:

- Instantiation of new objects in the ObjectFactory.
The scene almost empty. It is good for teamwork 
(helps to avoid conflicts).

- I use `GameObject.FindGameObjectWithTag(...).GetComponent<...>()` 
instead of `GameObject.FindObjectOfType.` It works much faster.
Also, I recommend to avoid using
`GetComponentsInChildren`. `FindObjectOfType` and `GetComponentsInChildren` 
go through full hierarchy of game objects and call `GetComponent` on every game object
  (This is very slow.).

- It makes sense to call methods-locators(`Find*`,`GetComponent*`)
as rare as possible. Ideally, only once in 
`Awake` or `Start`. 
If you place such calls in `Update`, it will kill performance of your game.

- Pay attention to the order of the game objects creation
and their initialization (1 -> 2 -> 3).
Because of the initialization order, binding
in `Start` methods (next frame after creation).
And that is why `ObjectFactory` is the last in 
the creation list. It must be initialized after the
other objects.


These points make the code a bit more complex, 
and make you to think about initialization order.
But it allows you to split your scene into independent
prefabs and work on the project in parallel manner.
You artefacts will be more stable with this approach 
than with direct binding.

# Dependency Injection

When you project is becoming bigger,
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
become very dirty after some time and it will be
hard to fix bugs.

The good solution in this case is using `Dependency Injection`. 
When you write a class you just declare, 
that it will need some other objects and, in runtime,
external system will provide these objects 
and notify you about it. 
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

*The full example you can find here `Assets/context`.*

I declare dependencies using the attribute `[Inject]`, `[SerializeField][NotEditable]`.
The *context* reads these attributes in the runtime and binds objects.

When the *context* resolves all dependencies, 
it calls a callback with the attribute `[AfterInject]`.

Pay attention to the superclass `SceneContextMonoBehaviour`.
It 'says' to the context to inject the dependencies
and add this object to context 
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
by some features (in this example - class).
- The dependency injector. `context.Inject`
finds all fields with the attribute `[Inject]`,
and inject all the necessary dependencies 
in the fields. It will wait if the target 
dependency is not available yet.
When it finishes, it will call a callback 
with the attribute `[AfterInject]`.

The *context* may live all alone,
but, often, it is better to bind to other parts 
of your system. 
In this example, the context is bound to the scene.
I highly recommend to see the implementation of `Context`
and use this approach in your projects.

This implementation is very small and rough
with a purpose to show you the main concepts of DI.
You can look at on the VContainer and ZInject as the
other mature implementations of this approach.

# [[Author]](/)


<script src='/assets/comments.js'></script>
