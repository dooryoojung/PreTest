using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class LaserPathTests
{
    // 먼 좌표를 기준점으로 잡아 실제 씬과 간섭하지 않게 테스트
    private static readonly Vector3 TestOrigin = new Vector3(1000f, 1000f, 1000f);

    private List<GameObject> _spawnedTempObj;

    // [SetUp]: [Test] 메서드가 실행되기 직전 자동으로 호출
    [SetUp]
    public void SetUp()
    {
        _spawnedTempObj = new List<GameObject>();
    }

    // [TearDown]: [Test] 메서드가 끝난 직후 자동으로 호출
    [TearDown]
    public void TearDown()
    {
        foreach (var obj in _spawnedTempObj)
        {
            // Destroy() -> 프레임 끝난 후 파괴 -> 이전 오브젝트 남아있을 수 있음
            Object.DestroyImmediate(obj);
        }
    }
    
    // 미러 생성
    private GameObject CreateMirror(Vector3 position)
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.transform.position = position;
        obj.AddComponent<Mirror>();
        _spawnedTempObj.Add(obj);

        // SyncTransforms()로 강제 동기화 후 Raycast
        Physics.SyncTransforms();
        return obj;
    }

    // 벽 생성
    private GameObject CreateWall(Vector3 position)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = position;
        _spawnedTempObj.Add(go);

        Physics.SyncTransforms();
        return go;
    }

    [Test]
    public void 충돌없음_레이저_최대거리_반사없음()
    {
        var result = new List<Vector3>();

        LaserPath.Compute(TestOrigin, Vector3.forward, 10f, 5, result, out var reflectionCount);

        Assert.AreEqual(0, reflectionCount);
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(TestOrigin + Vector3.forward * 10f, result[1]);
    }

    [Test]
    public void 벽_충돌_레이저_종료()
    {
        CreateWall(TestOrigin + Vector3.forward * 5f);
        var result = new List<Vector3>();

        LaserPath.Compute(TestOrigin, Vector3.forward, 10f, 5, result, out var reflectionCount);

        Assert.AreEqual(0, reflectionCount);
        Assert.AreEqual(2, result.Count);
        // 거리 비교로 실제 충돌을 검증 (벽과 충돌, 충돌없음 구분)
        Assert.Less(Vector3.Distance(TestOrigin, result[1]), 10f);
    }

    [Test]
    public void 미러_충돌_레이저_반사()
    {
        CreateMirror(TestOrigin + Vector3.forward * 5f);
        var result = new List<Vector3>();

        LaserPath.Compute(TestOrigin, Vector3.forward, 10f, 5, result, out var reflectionCount);

        Assert.AreEqual(1, reflectionCount);
    }

    [Test]
    public void 반사10회초과_반사_종료()
    {
        CreateMirror(TestOrigin + Vector3.forward * 5f);
        var result = new List<Vector3>();

        LaserPath.Compute(TestOrigin, Vector3.forward, 10f, 1, result, out var reflectionCount);

        Assert.AreEqual(0, reflectionCount);
        Assert.AreEqual(2, result.Count);
    }
}
