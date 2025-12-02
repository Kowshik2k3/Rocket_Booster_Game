using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SwipController : MonoBehaviour , IEndDragHandler
{
    [SerializeField] int maxPage;
    int currentPage;
    Vector3 targetPos;
    [SerializeField] Vector3 pageStep;
    [SerializeField] RectTransform levelPagesRect;

    [SerializeField] float tweentime;
    [SerializeField] LeanTweenType tweenType;
    float dragThreshold;

    [SerializeField] Image[] barImage;
    [SerializeField] Sprite barClosed, barOpen;

    [SerializeField] Button nextButton, previousButton;

    private void Awake()
    {
        currentPage = 1; // Start at page 1
        targetPos = levelPagesRect.localPosition; // Initialize targetPos to the current position
        dragThreshold = Screen.width / 15f; // Set drag threshold based on screen width
        UpdateBar();
        UpdateArrowButtons();
    }
    public void Next()
    {
        if(currentPage < maxPage)  // Check if we can go to the next page
        {
            currentPage++; // Increment current page
            targetPos += pageStep; // Move target position forward by one page step
            MovePage();
        }
    }

    public void Previous()
    {
        if(currentPage > 1)  // Check if we can go to the previous page
        {
            currentPage--; // Decrement current page
            targetPos -= pageStep; // Move target position back by one page step
            MovePage();
        }
    }

    void MovePage() // Move to the target position with tweening
    {
        levelPagesRect.LeanMoveLocal(targetPos, tweentime).setEase(tweenType);  // Use LeanTween for smooth movement
        UpdateBar();
        UpdateArrowButtons();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(Mathf.Abs(eventData.position.x-eventData.pressPosition.x) >  dragThreshold) // Check if the drag distance exceeds the threshold
        {
            if(eventData.position.x < eventData.pressPosition.x) Previous(); // Dragged left, go to previous page
            else Next(); // Dragged right, go to next page
        }
        else
        {
            MovePage(); // Not enough drag, snap back to the current page
        }
    }
    public void UpdateBar()
    {
        foreach( var item in barImage)
        {
            item.sprite = barClosed;
        }
        barImage[currentPage-1].sprite = barOpen;

    }

    void UpdateArrowButtons()
    {
        nextButton.interactable = true;
        previousButton.interactable = true;
        if (currentPage == 1) previousButton.interactable = false;
        else if (currentPage == maxPage) nextButton.interactable = false;
    }   
}
