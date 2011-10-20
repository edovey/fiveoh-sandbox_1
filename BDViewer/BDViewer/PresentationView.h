//
//  PresentationView.h
//  BDViewer
//
//  Created by Liz Dovey on 11-10-20.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface PresentationView : UIViewController <UITableViewDelegate, UITableViewDataSource>
{
    UITableView *dataTableView;
    NSString * parentId;
    NSArray * presentationArray;
}

@property (retain, nonatomic) NSString *parentId;
@property (retain, nonatomic) NSArray *presentationArray;
@property (retain, nonatomic) IBOutlet UITableView *dataTableView;

@end
