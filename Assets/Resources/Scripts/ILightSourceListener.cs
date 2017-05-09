public interface ILightSourceListener
{
    void SetLightActive(bool state);
    void SetLightSource(char lightSource);
    void ToggleLight();
}