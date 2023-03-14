# ShowIf attribute for Unity
ShowIf can conditionally hide properties in inspector.
## Example:
```cs
[System.Serializable]
public class Curve {
    public enum Kind {
        Line,
        Bezier,
    }
    
    public Kind kind;
    public Vector3 start;
    [ShowIf("IsBezier")] public Vector3 startControl;
    [ShowIf("IsBezier")] public Vector3 endControl;
    public Vector3 end;
    
    private bool IsBezier() => kind == Kind.Bezier;
}
```
