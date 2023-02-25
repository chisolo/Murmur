using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class IngredientHubView : MonoBehaviour
{
    [SerializeField] TextMesh _textMesh;

    public void SetCount(int count, bool init = false)
    {
        if(count > 0) {
            if(!gameObject.activeSelf) {
                gameObject.SetActive(true);
                transform.localScale = Vector3.zero;
                transform.DOScale(1f, 0.4f);
            }
            _textMesh.text = count.ToString();
        } else {
            if(gameObject.activeSelf) {
                if(init) gameObject.SetActive(false);
                else transform.DOScale(0f, 0.4f).OnComplete(() => {gameObject.SetActive(false);});
            }
        }
    }
}
