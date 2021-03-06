using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;

public class BallLauncher : MonoBehaviour
{

    private Vector3 _startDragPosition;
    private Vector3 _endDragPosition;
    private LaunchPreview _launchPreview;
    private List<Ball> _balls = new List<Ball>();
    private int _ballsReady;
    private BlockSpawner _blockSpawner;
    private ScoreManager _scoreManager;

    [SerializeField] private Ball ballPrefab;

    public int counter = 0;
    private void Awake()
    {
        _scoreManager = FindObjectOfType<ScoreManager>();
        _blockSpawner = FindObjectOfType<BlockSpawner>();
        _launchPreview = GetComponent<LaunchPreview>();
        CreateBall();
    }

    /**
     * Topları oluşturma ve skor ekleme
     */
    private void CreateBall()
    {
        _scoreManager.AddScore(1);
        counter++;

        var ball = Instantiate(ballPrefab);
        _balls.Add(ball);
        _ballsReady++;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.back * -10;
        
        if (Input.GetMouseButtonDown(0)) // ekrana tıklandı ise
        {
            StartDrag(worldPosition);
        }
        else if (Input.GetMouseButton(0))
        {
            ContinueDrag(worldPosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndDrag(worldPosition);
        }
    }

    /**
     * İlk ok çekildiğinde
     */
    private void StartDrag(Vector3 worldPosition)
    {
        _startDragPosition = worldPosition;
        _launchPreview.SetStartPoint(transform.position);
        
    }

    /**
     * Ok çekilmeye devam edildiğinde
     */
    private void ContinueDrag(Vector3 worldPosition)
    {
        _endDragPosition = worldPosition;
        Vector3 direction = _endDragPosition - _startDragPosition;
        _launchPreview.SetEndPoint(transform.position - direction);
    }
    
    private void EndDrag(Vector3 worldPosition)
    {
        StartCoroutine(LaunchBalls());
    }

    /**
     * Top oluşturma
     */
    private IEnumerator LaunchBalls()
    {
        Vector3 direction = _endDragPosition - _startDragPosition;
        direction.Normalize();
        
        foreach (var ball in _balls)
        {
            ball.transform.position = transform.position;
            ball.gameObject.SetActive(true);
            ball.GetComponent<Rigidbody2D>().AddForce(-direction);

            yield return new WaitForSeconds(0.1f);
        }

        _ballsReady = 0;

    }
    
    /**
     * Toplar başlangıca geri döndüğünde olacaklar.
     */
    public void ReturnBall()
    {
        _ballsReady++;
        if (_ballsReady == _balls.Count)
        {
            _blockSpawner.SpawnRowOfBlocks();
            CreateBall();
        }
    }
    
}
