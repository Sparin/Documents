export interface Order {
    _id: string
    firstName: string
    middleName: string
    lastName: string
    fruit: Fruit
    amount: number
    address: string
    isCallbackRequired: boolean
    eta: Date
}

export enum Fruit {
    Apple = "Apple",
    Orange = "Orange",
    Lemon = "Lemon",
    Banana = "Banana"
}

export interface SearchResponse<T> {
    totalItems: number
    totalPages: number
    currentPage: number
    items: Array<T>
}

export interface SearchOptions {
    fruits: Array<Fruit>
    minimumAmount: number
    maximumAmount: number
    address: string
    isCallbackRequired: boolean
    fromETA: Date
    untilETA: Date
    orderBy: string
    descending: boolean
}