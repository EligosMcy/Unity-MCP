using UnityEngine;
using DG.Tweening;

/// <summary>
/// 眼睛跟随脚本
/// 让蛇的眼睛能够跟随移动方向
/// </summary>
public class EyeFollow : MonoBehaviour
{
    [SerializeField]
    private Transform _leftEye; // 左眼 transform
    
    [SerializeField]
    private Transform _rightEye; // 右眼 transform
    
    [SerializeField]
    private float _eyeRotationSpeed = 5f; // 眼睛旋转速度
    
    private Vector3 _lastPosition; // 上一帧位置
    private Vector3 _moveDirection; // 移动方向
    
    private void Start()
    {
        _lastPosition = transform.position;
    }
    
    private void Update()
    {
        // 计算移动方向
        _moveDirection = transform.position - _lastPosition;
        
        // 如果有移动，旋转眼睛
        if (_moveDirection != Vector3.zero)
        {
            RotateEyes();
        }
        
        // 更新上一帧位置
        _lastPosition = transform.position;
    }
    
    /// <summary>
    /// 旋转眼睛以跟随移动方向
    /// </summary>
    private void RotateEyes()
    {
        // 计算目标旋转角度
        Quaternion targetRotation = Quaternion.LookRotation(_moveDirection, Vector3.up);
        
        // 使用 DOTween 平滑旋转眼睛
        if (_leftEye != null)
        {
            _leftEye.DORotate(targetRotation.eulerAngles, 0.2f).SetEase(Ease.OutQuad);
        }
        
        if (_rightEye != null)
        {
            _rightEye.DORotate(targetRotation.eulerAngles, 0.2f).SetEase(Ease.OutQuad);
        }
    }
}
