using UnityEngine;

[CreateAssetMenu(menuName = "Audio/SoundEvent")]
public class SoundEvent : ScriptableObject
{
    public FMODUnity.EventReference fmodEvent;

    public void Play(Vector3 position, int comboLevel = 1)
    {
        if (fmodEvent.IsNull) return;

        var instance = FMODUnity.RuntimeManager.CreateInstance(fmodEvent);

        instance.setParameterByName("Combo", comboLevel);

        instance.start();
        instance.release(); 
    }
}