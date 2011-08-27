//
//  PrototypeView.h
//  fiveoh-one
//
//  Created by Karl Schulze on 2011-08-26.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface PrototypeView : UIViewController 
{
    NSMutableArray *_documentArray;
    UITableView *dataTableView;
    UILabel *infoLabel;
    UILabel *modifiedDateLabel;
    UITextField *repositoryUrlTextField;
    UITextView *documentTextTextView;
}

@property (strong, nonatomic) NSMutableArray *documentArray;
@property (strong, nonatomic) IBOutlet UITableView *dataTableView;
@property (strong, nonatomic) IBOutlet UILabel *infoLabel;
@property (strong, nonatomic) IBOutlet UILabel *modifiedDateLabel;
@property (strong, nonatomic) IBOutlet UITextField *repositoryUrlTextField;
@property (strong, nonatomic) IBOutlet UITextView *documentTextTextView;

- (IBAction)updateRowAction:(id)sender;
- (IBAction)pushAction:(id)sender;
- (IBAction)pullAction:(id)sender;
- (IBAction)createAction:(id)sender;

@end
