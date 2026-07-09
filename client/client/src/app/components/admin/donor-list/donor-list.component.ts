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
import { MessageService, ConfirmationService } from 'primeng/api';
import { Donor } from '../../../models/donor';
import { DonorListService } from '../../../services/donor-list.service';
import * as pdfMake from 'pdfmake/build/pdfmake';
import * as pdfFonts from 'pdfmake/build/vfs_fonts';

// פונקציית עזר לקבלת ה-VFS של pdfMake (מחזירה אובייקט תמיד)
const getVfs = () => {
  try {
    const fonts = pdfFonts as any;
    return fonts?.pdfMake?.vfs || fonts?.vfs || (window as any).pdfMake?.vfs || {};
  } catch {
    return {};
  }
};

@Component({
  selector: 'app-donor-list',
  standalone: true,
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
    FormsModule
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './donor-list.component.html',
  styleUrl: './donor-list.component.css'
})
export class DonorListComponent implements OnInit {
  private donorListService = inject(DonorListService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);

  donorDialog: boolean = false;
  donors = signal<Donor[]>([]);
  donor: Donor = { id: 0, name: '', email: '', phone: '', address: '' };
  selectedDonors: Donor[] | null = null;
  submitted: boolean = false;

  ngOnInit(): void {
    this.loadDonors();
  }

  loadDonors(): void {
    this.donorListService.getAllDonors().subscribe({
      next: (data: Donor[]) => this.donors.set(data),
      error: (err: any) => {
        if (err?.status === 404) {
          this.donors.set([]);
        } else {
          console.error('Error loading donors:', err);
          this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: 'שגיאה בטעינת התורמים', life: 3000 });
        }
      }
    });
  }

  openNew(): void {
    this.donor = { id: 0, name: '', email: '', phone: '', address: '' };
    this.submitted = false;
    this.donorDialog = true;
  }

  editDonor(donor: Donor): void {
    this.donor = { ...donor };
    this.donorDialog = true;
  }


  hideDialog(): void {
    this.donorDialog = false;
    this.submitted = false;
  }

  deleteDonor(donor: Donor): void {
    this.confirmationService.confirm({
      message: 'האם אתה בטוח שברצונך למחוק את ' + donor.name + '?',
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
        this.donorListService.deleteDonor(donor.id).subscribe({
          next: () => {
            const updatedDonors = this.donors().filter((val) => val.id !== donor.id);
            this.donors.set(updatedDonors);
            
            this.donor = { id: 0, name: '', email: '', phone: '', address: '' };
            this.messageService.add({
              severity: 'success',
              summary: 'הצלחה',
              detail: 'התורם נמחק בהצלחה',
              life: 3000
            });
          },
          error: (err: any) => {
            console.error('Error deleting donor:', err);
            this.messageService.add({
              severity: 'error',
              summary: 'שגיאה',
              detail: 'שגיאה במחיקת התורם, יתכן ויש מתנות מקושרות לתורם זה',
              life: 3000
            });
          }
        });
      }
    });
  }

  findIndexById(id: number): number {
    const currentDonors = this.donors();
    return currentDonors.findIndex(donor => donor.id === id);
  }

// הוסף את הפונקציה הזו לקומפוננטה שלך
  
saveDonor(): void {
    this.submitted = true;

    if (this.donor.name?.trim() && this.donor.email?.trim() && 
        this.donor.phone?.trim() && this.donor.address?.trim()) {
      
      if (this.donor.id && this.donor.id > 0) {
        // עדכון תורם קיים
        this.donorListService.updateDonor(this.donor.id, this.donor).subscribe({
          next: (updatedDonor: Donor) => {
            
            // ✅ תיקון: אם השרת לא מחזיר id, שמור את המקורי
            if (!updatedDonor.id) {
              console.warn('⚠️ Server did not return id, preserving original:', this.donor.id);
              updatedDonor.id = this.donor.id;
            }
            
            const currentDonors = this.donors();
            const index = currentDonors.findIndex(d => d.id === this.donor.id);
            
            if (index !== -1) {
              const newDonors = [...currentDonors];
              newDonors[index] = updatedDonor;
              this.donors.set(newDonors);
            }
            
            this.messageService.add({
              severity: 'success',
              summary: 'הצלחה',
              detail: 'התורם עודכן בהצלחה',
              life: 3000
            });
            
            this.donorDialog = false;
            this.donor = { id: 0, name: '', email: '', phone: '', address: '' };
          },
          error: (err: any) => {
            console.error('Error updating donor:', err);
            this.messageService.add({
              severity: 'error',
              summary: 'שגיאה',
              detail: 'שגיאה בעדכון התורם',
              life: 3000
            });
          }
        });
      } else {
        // הוספת תורם חדש
        this.donorListService.createDonor(this.donor).subscribe({
          next: (newDonor: Donor) => {
            const currentDonors = this.donors();
            this.donors.set([...currentDonors, newDonor]);
            
            this.messageService.add({
              severity: 'success',
              summary: 'הצלחה',
              detail: 'התורם נוסף בהצלחה',
              life: 3000
            });
            
            this.donorDialog = false;
            this.donor = { id: 0, name: '', email: '', phone: '', address: '' };
          },
          error: (err: any) => {
            console.error('Error creating donor:', err);
            this.messageService.add({
              severity: 'error',
              summary: 'שגיאה',
              detail: 'שגיאה בהוספת התורם',
              life: 3000
            });
          }
        });
      }
    }
  }

  exportCSV(event: any): void {
    if (typeof window === 'undefined') return;

    const currentDonors = this.donors();
    if (!currentDonors || currentDonors.length === 0) {
      this.messageService.add({ severity: 'warn', summary: 'אין נתונים', detail: 'אין תורמים לייצוא', life: 3000 });
      return;
    }

    const fonts = {
      HebrewFont: {
        normal: 'https://raw.githubusercontent.com/google/fonts/main/ofl/heebo/Heebo%5Bwght%5D.ttf',
        bold: 'https://raw.githubusercontent.com/google/fonts/main/ofl/heebo/Heebo%5Bwght%5D.ttf',
        italics: 'https://raw.githubusercontent.com/google/fonts/main/ofl/heebo/Heebo%5Bwght%5D.ttf',
        bolditalics: 'https://raw.githubusercontent.com/google/fonts/main/ofl/heebo/Heebo%5Bwght%5D.ttf'
      }
    };

    const documentDefinition: any = {
      content: [
        { text: this.fixTextDirection('רשימת תורמים - מכירה סינית'), style: 'header' },
        {
          table: {
            headerRows: 1,
            widths: ['*', '*', 'auto', '*'],
            body: [
              [
                { text: 'כתובת', style: 'tableHeader' },
                { text: 'אימייל', style: 'tableHeader' },
                { text: 'טלפון', style: 'tableHeader' },
                { text: 'שם התורם', style: 'tableHeader' }
              ],
              ...currentDonors.map(d => [
                { text: this.fixTextDirection(d.address || ''), alignment: 'right' },
                { text: d.email || '', alignment: 'left' },
                { text: d.phone || '', alignment: 'center' },
                { text: this.fixTextDirection(d.name || ''), alignment: 'right' }
              ])
            ]
          },
          layout: 'lightHorizontalLines'
        }
      ],
      rtl: true,
      defaultStyle: { font: 'HebrewFont', alignment: 'right', fontSize: 10 },
      styles: {
        header: { fontSize: 22, bold: true, alignment: 'center', margin: [0, 0, 0, 20] },
        tableHeader: { bold: true, fontSize: 12, color: 'white', fillColor: '#10b981', alignment: 'right' }
      }
    };

    try {
      const vfs = getVfs();
      const pMake = (pdfMake as any).default || pdfMake;
      pMake.vfs = vfs;
      pMake.fonts = fonts;
      pMake.createPdf(documentDefinition, {}, fonts, vfs).download('donors_list.pdf');
      this.messageService.add({ severity: 'success', summary: 'ייצוא', detail: 'הקובץ ירד בהצלחה', life: 3000 });
    } catch (error) {
      console.error('שגיאה ביצירת PDF:', error);
      this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: 'ייצוא הקובץ נכשל', life: 3000 });
    }
  }

  fixTextDirection(text: string): string {
    if (!text) return '';
    const cleanText = String(text).trim().replace(/\s+/g, ' ');
    const hasHebrew = /[֐-׿]/.test(cleanText);
    if (hasHebrew) {
      return cleanText.split(' ').reverse().join('  ');
    }
    return cleanText;
  }
}