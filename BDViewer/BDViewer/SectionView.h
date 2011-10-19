//
//  SectionView.h
//  BDViewer
//
//  Created by Liz Dovey on 11-10-19.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface SectionView : UIViewController <UITableViewDataSource, UITableViewDelegate>
{
    NSArray *sectionArray;
    UITableView *dataTableView;
    NSString *parentId;
}

@property (nonatomic, retain) IBOutlet UITableView *dataTableView;
@property (nonatomic, retain) NSString *parentId;
@property (nonatomic, retain) NSArray *sectionArray;

@end
