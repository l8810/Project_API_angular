import { FileUploadService } from '../../../services/file-upload.service';
import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { TableModule } from 'primeng/table';
import { ToastModule } from 'primeng/toast';
import { ToolbarModule } from 'primeng/toolbar';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { MessageService, ConfirmationService } from 'primeng/api';
import { Gift } from '../../../models/gift';
import { Donor } from '../../../models/donor';
import { Category } from '../../../models/category';
import { GiftListService } from '../../../services/gift-list.service';
import { DonorListService } from '../../../services/donor-list.service';
import { CategoryListService } from '../../../services/category-list.service';
import { environment } from '../../../enviorments/enviorment';
import { TagModule } from 'primeng/tag';

@Component({
  selector: 'app-gift-list',
  imports: [
    CommonModule,
    ButtonModule,
    ConfirmDialogModule,
    DialogModule,
    IconFieldModule,
    InputIconModule,
    TableModule,
    ToastModule,
    ToolbarModule,
    InputTextModule,
    SelectModule,
    FormsModule,
    TagModule
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './gift-list.component.html',
  styleUrl: './gift-list.component.css',
})
export class GiftListComponent implements OnInit {
  private giftListService = inject(GiftListService);
  private donorService = inject(DonorListService);
  private categoryService = inject(CategoryListService);
  private fileUploadService = inject(FileUploadService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);

  giftDialog: boolean = false;
  gifts = signal<Gift[]>([]);
  donors = signal<Donor[]>([]);
  categories = signal<Category[]>([]);

  gift: Gift = {
    id: 0,
    name: '',
    description: '',
    donorId: 0,
    donorName: '',
    price: 0,
    categoryId: 0,
    categoryName: '',
    picture: ''
  };

  selectedGifts: Gift[] | null = null;
  submitted: boolean = false;
  selectedFile: File | null = null;
  isUploading: boolean = false;

  ngOnInit(): void {
    this.loadGifts();
    this.loadDonors();
    this.loadCategories();
  }

  loadGifts(): void {
    this.giftListService.getAllGifts().subscribe({
      next: (data: Gift[]) => this.gifts.set(data),
      error: (err: any) => {
        console.error('Error loading gifts:', err);
        this.messageService.add({
          severity: 'error',
          summary: 'שגיאה',
          detail: 'שגיאה בטעינת המתנות',
          life: 3000
        });
      }
    });
  }

  loadDonors(): void {
    this.donorService.getAllDonors().subscribe({
      next: (data: Donor[]) => this.donors.set(data),
      error: (err: any) => {
        console.error('Error loading donors:', err);
        this.messageService.add({
          severity: 'error',
          summary: 'שגיאה',
          detail: 'שגיאה בטעינת התורמים',
          life: 3000
        });
      }
    });
  }

  loadCategories(): void {
    this.categoryService.getAllCategories().subscribe({
      next: (data: Category[]) => this.categories.set(data),
      error: (err: any) => {
        console.error('Error loading categories:', err);
        this.messageService.add({
          severity: 'error',
          summary: 'שגיאה',
          detail: 'שגיאה בטעינת הקטגוריות',
          life: 3000
        });
      }
    });
  }

  openNew(): void {
    this.gift = {
      id: 0,
      name: '',
      description: '',
      donorId: 0,
      donorName: '',
      price: 0,
      categoryId: 0,
      categoryName: '',
      picture: ''
    };
    this.submitted = false;
    this.giftDialog = true;
  }

  editGift(gift: Gift): void {
    console.log('Editing gift:', gift);
    console.log('Available donors:', this.donors());
    this.gift = { ...gift };
    this.giftDialog = true;
  }

  hideDialog(): void {
    this.giftDialog = false;
    this.submitted = false;
    this.selectedFile = null;
  }

  deleteGift(gift: Gift): void {
    this.confirmationService.confirm({
      message: 'האם אתה בטוח שברצונך למחוק את ' + gift.name + '?',
      header: 'אישור מחיקה',
      icon: 'pi pi-exclamation-triangle',
      rejectButtonProps: {
        label: 'לא',
        severity: 'secondary',
        variant: 'text'
      },
      acceptButtonProps: {
        severity: 'danger',
        label: 'כן'
      },
      accept: () => {
        this.giftListService.deleteGift(gift.id).subscribe({
          next: () => {
            const updatedGifts = this.gifts().filter((val) => val.id !== gift.id);
            this.gifts.set(updatedGifts);

            this.gift = {
              id: 0,
              name: '',
              description: '',
              donorId: 0,
              donorName: '',
              price: 0,
              categoryId: 0,
              categoryName: '',
              picture: ''
            };
            this.messageService.add({
              severity: 'success',
              summary: 'הצלחה',
              detail: 'מתנה נמחקה בהצלחה',
              life: 3000
            });
          },
          error: (err: any) => {
            console.error('Error deleting gift:', err);
            this.messageService.add({
              severity: 'error',
              summary: 'שגיאה',
              detail: 'שגיאה במחיקת המתנה',
              life: 3000
            });
          }
        });
      }
    });
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      // בדוק שזה תמונה
      if (!file.type.startsWith('image/')) {
        this.messageService.add({
          severity: 'error',
          summary: 'שגיאה',
          detail: 'יש להעלות קובץ תמונה בלבד',
          life: 3000
        });
        return;
      }

      this.selectedFile = file;
      this.isUploading = true;

      // העלה את הקובץ מיד
      this.fileUploadService.uploadImage(file).subscribe({
        next: (response) => {
          this.gift.picture = response.fileName;
          this.isUploading = false;
          this.messageService.add({
            severity: 'success',
            summary: 'הצלחה',
            detail: 'תמונה הועלתה בהצלחה',
            life: 3000
          });
        },
        error: (err) => {
          console.error('Error uploading image:', err);
          this.isUploading = false;
          this.messageService.add({
            severity: 'error',
            summary: 'שגיאה',
            detail: 'שגיאה בהעלאת התמונה',
            life: 3000
          });
        }
      });
    }
  }

  saveGift(): void {
    this.submitted = true;

    console.log('Gift object before save:', this.gift);
    console.log('donorId:', this.gift.donorId, 'type:', typeof this.gift.donorId);
    console.log('categoryId:', this.gift.categoryId, 'type:', typeof this.gift.categoryId);

    if (this.gift.name?.trim() && this.gift.donorId &&
      this.gift.categoryId && this.gift.picture?.trim()) {

      if (this.gift.id && this.gift.id > 0) {
        // עדכון מתנה קיימת
        this.giftListService.updateGift(this.gift.id, this.gift).subscribe({
          next: (updatedGift: Gift) => {
            // טען מחדש את כל הרשימה
            this.loadGifts();

            this.messageService.add({
              severity: 'success',
              summary: 'הצלחה',
              detail: 'מתנה עודכנה בהצלחה',
              life: 3000
            });

            this.giftDialog = false;
            this.gift = {
              id: 0,
              name: '',
              description: '',
              donorId: 0,
              donorName: '',
              price: 0,
              categoryId: 0,
              categoryName: '',
              picture: ''
            };
          },
          error: (err: any) => {
            console.error('Error updating gift:', err);
            console.error('Error details:', err.error);
            this.messageService.add({
              severity: 'error',
              summary: 'שגיאה',
              detail: 'שגיאה בעדכון המתנה',
              life: 3000
            });
          }
        });
      } else {
        // הוספת מתנה חדשה
        this.giftListService.createGift(this.gift).subscribe({
          next: (newGift: Gift) => {
            // במקום להוסיף ידנית, פשוט טען מחדש את כל הרשימה
            this.loadGifts();

            this.messageService.add({
              severity: 'success',
              summary: 'הצלחה',
              detail: 'מתנה נוספה בהצלחה',
              life: 3000
            });

            this.giftDialog = false;
            this.gift = {
              id: 0,
              name: '',
              description: '',
              donorId: 0,
              donorName: '',
              price: 0,
              categoryId: 0,
              categoryName: '',
              picture: ''
            };
          },
          error: (err: any) => {
            console.error('Error creating gift:', err);
            console.error('Error details:', err.error);
            console.error('Validation errors:', err.error?.errors);
            this.messageService.add({
              severity: 'error',
              summary: 'שגיאה',
              detail: 'שגיאה בהוספת מתנה',
              life: 3000
            });
          }
        });
      }
    } else {
      console.log('Validation failed!');
      console.log('name:', this.gift.name);
      console.log('donorId:', this.gift.donorId);
      console.log('categoryId:', this.gift.categoryId);
      console.log('picture:', this.gift.picture);
    }
  }

  getImageUrl(fileName: string): string {
    if (!fileName) return 'assets/placeholder.png';
    
    if (fileName.startsWith('http')) return fileName;
    
    return `${environment.apiUrl}/uploads/gifts/${fileName}`;
  }

  exportCSV(event: any): void {
    const currentGifts = this.gifts();
    const csvData = currentGifts.map(d => ({
      'שם': d.name,
      'תיאור': d.description,
      'תורם': d.donorName,
      'מחיר': d.price,
      'קטגוריה': d.categoryName,
      'תמונה': d.picture
    }));

    console.log('Export CSV:', csvData);
    this.messageService.add({
      severity: 'info',
      summary: 'ייצוא',
      detail: 'פונקציונליות ייצוא תתווסף בקרוב',
      life: 3000
    });
  }
  viewImage(fileName: string): void {
    if (fileName) {
      window.open(this.getImageUrl(fileName), '_blank');
    }
  }
}