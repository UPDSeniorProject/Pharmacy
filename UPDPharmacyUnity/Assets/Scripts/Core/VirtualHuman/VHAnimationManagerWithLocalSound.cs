using UnityEngine;
using System.Collections;

public class VHAnimationManagerWithLocalSound : VHAnimationManager {

    public AudioClip[] audioclips;
	
	protected VHSoundManager SoundManager;

    protected override void Start()
    {
        base.Start();

        PlayLipSyncButton.ClearButtonPressedEventHandlers();
        PlayLipSyncButton.ButtonPressed += new ButtonPressedEventHandler(PlayLipSync_ButtonPressed);
        
		SoundManager = this.gameObject.GetComponent<VHSoundManager>();
		
    }

    protected override void PlayLipSync_ButtonPressed(RenButton btn, ButtonPressedEventArgs args)
    {
        if (args.button == MouseButton.MOUSE_LEFT)
        {
            string animation = ComboBox.SelectedItem.text;
            string animWithoutDefault = animation.Substring(animation.IndexOf('_') + 1);
            AudioClip audioclip = null;

            foreach (AudioClip c in audioclips)
            {
                if (c.name.ToLower().Equals(animWithoutDefault))
                {
                    audioclip = c;
                    break;
                }
            }


			if(audioclip != null) {
				SoundManager.EnqueueLipSync(new LipSyncInfo(audioclip, animation));
			}else
				PlayLipSync(animation, audioclip);
        }
    }

	
}
