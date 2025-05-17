using System.Collections;
using UnityEngine;

public class DamageControl : MonoBehaviour
{
    // Расстояние, на которое перемещается одна вершина за одно взаимодействие
    [SerializeField]
    private float _maxMoveVertex = 1.0f;

    // Максимальная сила столкновения
    [SerializeField]
    private float _maxCollisionPower = 50.0f;

    // Максимальная скорость столкновения
    [SerializeField]
    private float _maxSpeedY = 0.5f;

    // Диапазон разрушения
    [SerializeField]
    private float _destructionRange = 1f;

    // Угол смещения вершин
    [SerializeField]
    private float _impactManipulator = 0.5f;

    // Оптимальная сетка объекта
    [SerializeField]
    private MeshFilter[] _optionalMesh;
    // Сетка объекта
    private MeshFilter[] _meshfilters;
    // Квадрат диапазона разрушения
    private float _sqrDestructionRange;

    // Точка столкновения
    private Vector3 _collisionPoint;
    // Сила столкновения
    private float _collisionPower;
    public void Start()
    {
        if (_optionalMesh.Length > 0)
        { _meshfilters = _optionalMesh; }
        else
        { _meshfilters = GetComponentsInChildren<MeshFilter>(); }
        // Сохраняем квадрат диапазона разрушения
        _sqrDestructionRange = _destructionRange * _destructionRange;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Получение скорости столкновения
        Vector3 collisionRelVel = collision.relativeVelocity;
        // Ограничиваем максимальную скорость по Y
        collisionRelVel.y *= _maxSpeedY;
        // Проверка количества точкек контакта
        if (collision.contacts.Length > 0)
        {
            // Смещение точки взаимодействия по точке контакта
            _collisionPoint = transform.position - collision.contacts[0].point;
            // Сохранения силы столкновения
            _collisionPower = collisionRelVel.magnitude * Vector3.Dot(collision.contacts[0].normal, _collisionPoint.normalized);
            // Если смещение точки больше 1
            if (_collisionPoint.magnitude > 1.0f )
            {
                
                // Деформируем сетку по заданным точкам
                OnMeshDeform(collision.contacts[0].point, 
                            Mathf.Clamp01(_collisionPower / _maxCollisionPower));
            }
        }
    }
    private void OnMeshDeform(Vector3 originPosition, float force)
    {
        // Округление силы (0,1)
        force = Mathf.Clamp01(force);
        // Перерасчет каждой сетки
        for (int i = 0; i < _meshfilters.Length; i++)
        {
            // Получение вершин
            Vector3[] vertices = _meshfilters[i].mesh.vertices;
            // Перерасчет каждой вершины
            for (int j = 0; j < vertices.Length; j++)
            {
                // Произведение параметров вершин
                Vector3 scaledVertex = Vector3.Scale(vertices[j], transform.localScale);
                // Перерасчет позиции вершины в мировых координатах
                Vector3 vertexWorldPosition = _meshfilters[i].transform.position + (_meshfilters[i].transform.rotation * scaledVertex);
                // Перерасчет истока вершины
                Vector3 vertexOriginToMe = vertexWorldPosition - originPosition;
                // Перерасчет центра вершины 
                Vector3 vertexToCenter = transform.position - vertexWorldPosition;
                // Обнуление центра вершины по y
                vertexToCenter.y = 0.0f;
                // Если квадрат центра вершины меньше диапазона разрушения
                if (vertexOriginToMe.sqrMagnitude < _sqrDestructionRange) 
                {
                    // Получаем глубину разрушения
                    float distance = Mathf.Clamp01(vertexOriginToMe.sqrMagnitude / _sqrDestructionRange);
                    // Добавляем силу разрушения
                    float moveDelta = force * (1.0f - distance) * _maxMoveVertex;
                    // Деформируем точку по углу смещения
                    Vector3 move = Vector3.Slerp(vertexOriginToMe, vertexToCenter, _impactManipulator).normalized * moveDelta;
                    // Вращаем точку
                    vertices[j] += Quaternion.Inverse(transform.rotation) * move;
                }
            }
            // Обновление вершин
            _meshfilters[i].mesh.vertices = vertices;
            // Обновление сетки
            _meshfilters[i].mesh.RecalculateBounds();
        }
    }
}
