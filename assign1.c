#include <stdio.h>
#include <stdlib.h>

int main() {
    int a, *ip, arr[3];

    // 1. 기본 배정문 실습
    a = 10;
    ip = &a;
    arr[0] = 10;

    printf("--- 기본 배정문 ---\n");
    printf("a의 값: %d\n", a);
    printf("ip가 가리키는 값(*ip): %d\n", *ip);
    printf("arr[0]의 값: %d\n\n", arr[0]);

    // 2. 동적 할당과 포인터 배정문 (슬라이드 4페이지 실습)
    int *ptr_a = (int *)malloc(sizeof(int));
    int *ptr_b = (int *)malloc(sizeof(int));

    *ptr_a = 1;
    *ptr_b = 2; // 두 변수의 차이를 확실히 보기 위해 2로 넣어볼게요

    printf("--- 포인터 배정문 (얕은 복사 전) ---\n");
    printf("ptr_a가 가리키는 값: %d, 주소: %p\n", *ptr_a, ptr_a);
    printf("ptr_b가 가리키는 값: %d, 주소: %p\n\n", *ptr_b, ptr_b);

    ptr_a = ptr_b; // 얕은 복사: 주소값 자체를 복사

    printf("--- 포인터 배정문 (ptr_a = ptr_b 실행 후) ---\n");
    printf("ptr_a가 가리키는 값: %d, 주소: %p\n", *ptr_a, ptr_a);
    printf("ptr_b가 가리키는 값: %d, 주소: %p\n", *ptr_b, ptr_b);

    return 0;
}