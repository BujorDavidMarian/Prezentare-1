using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UISpriteAnimation : MonoBehaviour
{
    public Image m_Image;
    public Sprite[] m_SpriteArray;
    private int m_IndexSprite = 0;
    private Coroutine m_CorotineAnim;

    void Start()
    {
        if (m_SpriteArray.Length > 0)
        {
            m_CorotineAnim = StartCoroutine(Func_PlayAnimUI());
        }
    }

    IEnumerator Func_PlayAnimUI()
    {
        while (true)
        {
            float waitTime = (m_IndexSprite == 0) ? 10f : 0.2f; 
            m_Image.sprite = m_SpriteArray[m_IndexSprite];
            yield return new WaitForSeconds(waitTime);

            m_IndexSprite = (m_IndexSprite + 1) % m_SpriteArray.Length;
        }
    }
}
