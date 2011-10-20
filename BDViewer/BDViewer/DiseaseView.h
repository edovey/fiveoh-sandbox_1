//
//  DiseaseView.h
//  BDViewer
//
//  Created by Liz Dovey on 11-10-19.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface DiseaseView : UIViewController <UITableViewDelegate, UITableViewDataSource>
{
    NSArray * diseaseArray;
    NSString *parentId;
    UITableView * dataTableView;
}

@property (nonatomic, retain) NSArray * diseaseArray;
@property (retain, nonatomic) NSString *parentId;
@property (retain, nonatomic) IBOutlet UITableView *dataTableView;
@end
