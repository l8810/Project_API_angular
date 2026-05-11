import { ProductPurches } from "./ProductPurches";

export interface Order {
    id: number;
    userId:number;
    userName:string;
    orderDate:Date;
    listOrder:ProductPurches[];
  }